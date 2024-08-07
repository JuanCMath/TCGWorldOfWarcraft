using UnityEngine;
using UnityEngine.EventSystems;
using Enums;
using System.Text.RegularExpressions;
using System.Linq;
using Compiler;

public class DropZoneGeneric : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
        // Numero de cartas maximo por panel
        private int maxCards;
        private bool playerCanPlaceCard;
        private string allowedCards;
        private type[] allowedCardsType;

        public void OnDrop(PointerEventData eventData) //Metodo que se inicia cuando un onjeto se dropea en la zona
        {           
            GameObject dropedCard = eventData.pointerDrag;

            Draggable draggedComponent = dropedCard.GetComponent<Draggable>();
            //Si tienes algo agarrado y Si el panel no esta lleno y Si la carta es del mismo tipo del panel entonces dropear
            if (draggedComponent != null)
            {
                if (CardCanBePlaced(dropedCard))
                {
                    draggedComponent.parentToReturnTo = this.transform;
                    GameObject.Find("Game Manager").GetComponent<GameManager>().numberOfActionsAvailable --;

                    ApplyEffect(dropedCard);  
                }      
            }  
        }
        
        public void ApplyEffect(GameObject dropedCard)
        {
                Evaluator evaluator = new Evaluator();
                evaluator.Evaluate(dropedCard.GetComponent<Card>().cardEffect);
        }

        public bool CardCanBePlaced(GameObject dropedCard)
        {
            if (maxCards >= gameObject.transform.childCount                                              &&
                GameObject.Find("Game Manager").GetComponent<GameManager>().numberOfActionsAvailable > 0 &&
                playerCanPlaceCard == true                                                               &&
                allowedCardsType.Contains(dropedCard.GetComponent<Card>().cardType)                      &&
                dropedCard.GetComponent<Card>().cardSlot.Contains(allowedCards))
            {
                if (dropedCard.transform.tag == "Card Player1" && GameManager.player1 == true || dropedCard.transform.tag == "Card Player2" && GameManager.player2 == true)
                    return true;
                else
                    return false;
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

        

    public void Start()
    {
        if (gameObject.transform.name == "Melee Zone" || 
            gameObject.transform.name == "Range Zone" || 
            gameObject.transform.name == "Siege Zone" ||
            gameObject.transform.name == "Melee Zone 2" || 
            gameObject.transform.name == "Range Zone 2" || 
            gameObject.transform.name == "Siege Zone 2")
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
        if (GameManager.player1 == true && gameObject.transform.parent.name == "Field p1")
        {
            playerCanPlaceCard = true;
        }
        else if (GameManager.player2 == true && gameObject.transform.parent.name == "Field p2")
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
        allowedCards = match.Groups[1].Success ? new string ("Melee") :
                          match.Groups[2].Success ? new string ("Range") :
                          match.Groups[3].Success ? new string ("Siege") :
                          match.Groups[4].Success ? new string ("Clima") :
                          match.Groups[5].Success ? new string ("Aumento") : 
                          null;

        allowedCardsType = match.Groups[1].Success ? new type[] {type.Unidad, type.Señuelo} :
                          match.Groups[2].Success ? new type[] {type.Unidad, type.Señuelo} :
                          match.Groups[3].Success ? new type[] {type.Unidad, type.Señuelo} :
                          match.Groups[4].Success ? new type[] {type.Clima, type.Despeje} :
                          match.Groups[5].Success ? new type[] {type.Aumento} : 
                          null;
        }
        else
        {
            Debug.Log("El nombre no contiene las letras 'M', 'R', 'S', 'C' o 'A'.");
        }
    }
}
