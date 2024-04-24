using System.Collections;
using System.Collections.Generic;
using Enums;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;

public class DropZoneMP2 : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
        // Numero de cartas maximo por panel
        private int maxCards = 5;  
                                    
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
            if (draggedComponent != null && maxCards >= this.transform.childCount)
            {
                if (GameObject.Find("Game Manager").GetComponent<GameManager>().numberOfActionsAvailable > 0)
                {
                    if (GameManager.player2 == true)
                    {
                        if (dropedCard.GetComponent<Card>().cardSlot == slot.M || dropedCard.GetComponent<Card>().cardSlot == slot.MR || dropedCard.GetComponent<Card>().cardSlot == slot.MS || dropedCard.GetComponent<Card>().cardSlot == slot.MRS)
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
            }  
        }
}
