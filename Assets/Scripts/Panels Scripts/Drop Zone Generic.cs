using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Enums;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using System.Linq;
using System.Runtime.InteropServices;

public class DropZoneGeneric : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
        // Numero de cartas maximo por panel
        private int maxCards;
        private bool playerCanPlaceCard;
        private bool cardCanBePlaced;
        private type allowedTypeCard;
        private slot[] allowedCards;

        public bool CardCanBePlaced(GameObject dropedCard)
        {
            if (maxCards >= gameObject.transform.childCount)
            {
                if (GameObject.Find("Game Manager").GetComponent<GameManager>().numberOfActionsAvailable > 0)
                {
                    if (playerCanPlaceCard == true)
                    {
                        if (dropedCard.GetComponent<Card>().cardType == allowedTypeCard)
                        {
                            if (allowedCards.Contains(dropedCard.GetComponent<Card>().cardSlot))
                            {
                                return true;
                            }
                            else return false;
                        }
                        else return false;
                    }
                    else return false;
                }
                else return false;
            }
            else return false; 
        }
                         
        public void OnPointerEnter(PointerEventData eventData) //Metodo que se inicia cuando el puntero entra en la zona, evenData almacena los datos de mi puntero de clase PointerEventData
        {
            if (eventData.pointerDrag == null)                 //Si no tengo nada agarrado
            {
                return;
            }
            Draggable  draggedComponent = eventData.pointerDrag.GetComponent<Draggable>();  // Obteniendo el componente Draggable del objeto que esta siendo agarrado
            if (draggedComponent != null)
            {
                draggedComponent.placeholderParent = this.transform;
            }
        }

        public void OnPointerExit(PointerEventData eventData) //Metodo que se inicia cuando el puntero sale de la zona
        {
            if (eventData.pointerDrag == null)
            {
                return;
            }
            Draggable  draggedComponent = eventData.pointerDrag.GetComponent<Draggable>();
            if (draggedComponent != null && draggedComponent.placeholderParent==this.transform)
            {
                draggedComponent.placeholderParent = draggedComponent.parentToReturnTo;
            }
        }

        public void OnDrop(PointerEventData eventData) //Metodo que se inicia cuando un onjeto se dropea en la zona
        {           
            GameObject dropedCard = eventData.pointerDrag;
            GameObject dropedPanel = gameObject;

            Draggable draggedComponent = dropedCard.GetComponent<Draggable>();
            //Si tienes algo agarrado y Si el panel no esta lleno y Si la carta es del mismo tipo del panel entonces dropear
            if (draggedComponent != null)
            {
                if (CardCanBePlaced(dropedCard))
                {
                    draggedComponent.parentToReturnTo = this.transform;
                    GameObject.Find("Game Manager").GetComponent<GameManager>().numberOfActionsAvailable --;

                    if (dropedCard.GetComponent<Card>().cardType == type.Señuelo)
                    {
                        GameObject.Find("Effect Manager").GetComponent<EffectsManager>().BaitEffect(dropedPanel);
                    }
                    else if (dropedCard.GetComponent<Card>().cardType == type.Unidad && dropedCard.GetComponent<Card>().isHero == false)
                    {
                        if (dropedCard.GetComponent<Card>().cardDescription == "Roba una Carta")
                        {
                            EffectsManager.DrawACard();
                        }
                        else if (dropedCard.GetComponent<Card>().cardDescription == "Destruye la carta con menor poder del enemigo")
                        {
                            EffectsManager.DestroyLowerPowerCardOnOponent();
                        }
                        else if (dropedCard.GetComponent<Card>().cardDescription == "Destruye la criatura con mas poder en el campo")
                        {
                            EffectsManager.DestroyHighestPowerCardOnField(dropedCard);
                        }
                        else if (dropedCard.GetComponent<Card>().cardDescription == "Esta criatura es mas fuerte en manada, multiplica su ataque por la cantidad de critaturas iguales en el campo")
                        {
                            EffectsManager.MultAttackPower(dropedCard);
                        }
                    }
                }      
            }  
        }

    public void Start()
    {
        if (gameObject.transform.name == "Melee Zone" || gameObject.transform.name == "Range Zone" || gameObject.transform.name == "Siege Zone")
        {
            maxCards = 5;
        }
        else if (gameObject.transform.name == "Clima Zone")
        {
            maxCards = 3;
        }
        else
        {
            maxCards = 1;
        }
        LoadingName();
    }

    public void Update()
    {
        if (GameManager.player1 == true && gameObject.transform.parent.name == "Panels p1")
        {
            playerCanPlaceCard = true;
        }
        else if (GameManager.player2 == true && gameObject.transform.parent.name == "Panels p2")
        {
            playerCanPlaceCard = true;
        }
    }

    public void LoadingName()
    {
        string nombre = gameObject.transform.name;

        // Patrón regex para buscar las letras 'M', 'R', 'S', 'C' o 'A' (mayúsculas o minúsculas)
        string patron = @"(M)|(R)|(S)|(C)|(A)";

        // Crear el objeto Regex con el patrón
        Regex regex = new Regex(patron, RegexOptions.IgnoreCase);

        // Verificar si el nombre contiene las letras ''M', 'R', 'S', 'C' o 'A'
        Match match = regex.Match(nombre);

        if (match.Success)
        {
            allowedCards = match.Groups[1].Success ? new slot[] {slot.M, slot.MR, slot.MS, slot.MRS} :
                          match.Groups[2].Success ? new slot[] {slot.R, slot.MR, slot.RS, slot.MRS} :
                          match.Groups[3].Success ? new slot[] {slot.S, slot.MS, slot.RS, slot.MRS} :
                          match.Groups[4].Success ? new slot[] {slot.X} :
                          match.Groups[5].Success ? new slot[] {slot.X} : 
                          null;

        if (match.Groups[1].Success || //'M'
            match.Groups[2].Success || //'R'
            match.Groups[3].Success)   //'S'
            {
                allowedTypeCard = type.Unidad;
            }
            else if (match.Groups[4].Success) // 'C'
            {
                allowedTypeCard = type.Clima;
            }
            else if (match.Groups[5].Success) // 'A'
            {
                allowedTypeCard = type.Aumento;
            }
        }
        else
        {
            Debug.Log("El nombre no contiene las letras 'M', 'R', 'S', 'C' o 'A'.");
        }
    }
}
