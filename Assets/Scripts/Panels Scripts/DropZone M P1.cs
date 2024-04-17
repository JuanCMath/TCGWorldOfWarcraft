using System.Collections;
using System.Collections.Generic;
using Enums;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;

public class DropZoneMP1 : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
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
            Debug.Log(eventData.pointerDrag.name + "OnDrop to " + gameObject.name);
            
            GameObject dropedCard = eventData.pointerDrag;

            Draggable draggedComponentOfDropedCard = dropedCard.GetComponent<Draggable>();
            //Si tienes algo agarrado y Si el panel no esta lleno y Si la carta es del mismo tipo del panel entonces dropear
            if (draggedComponentOfDropedCard != null && GameManager.player1 == true && maxCards >= this.transform.childCount)
            {
                if (GameObject.Find("Game Manager").GetComponent<GameManager>().numberOfActionsAvailable > 0)
                {
                    if (GameManager.player1 == true)
                    {
                        if (dropedCard.GetComponent<Card>().cardSlot == slot.M || dropedCard.GetComponent<Card>().cardSlot == slot.MR || dropedCard.GetComponent<Card>().cardSlot == slot.MS || dropedCard.GetComponent<Card>().cardSlot == slot.MRS)
                        {
                            draggedComponentOfDropedCard.parentToReturnTo = this.transform;
                            GameObject.Find("Game Manager").GetComponent<GameManager>().numberOfActionsAvailable --;

                            if (dropedCard.GetComponent<Card>().cardType == type.Señuelo)
                            {
                                GameObject.Find("Effect Manager").GetComponent<EffectsManager>().SeñueloEffect(gameObject);
                            }
                            else if (dropedCard.GetComponent<Card>().cardType == type.Unidad && dropedCard.GetComponent<Card>().isHero == false)
                            {

                            }
                        }      
                    } 
                }
            }  
        }
}
