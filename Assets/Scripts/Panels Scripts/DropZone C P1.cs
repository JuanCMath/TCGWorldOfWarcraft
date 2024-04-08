using System.Collections;
using System.Collections.Generic;
using Enums;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;

public class DropZoneCP1 : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
        private int maxCards = 2;                              // Numero de cartas maximo por panel
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

            Draggable  draggedComponent = eventData.pointerDrag.GetComponent<Draggable>();
            //Si tienes algo agarrado y Si el panel no esta lleno y Si la carta es del mismo tipo del panel entonces dropear
            if (draggedComponent != null && GameManager.player1 == true && maxCards >= this.transform.childCount)
            {
                if (GameManager.player1 == true)
                {
                    if (eventData.pointerDrag.GetComponent<Card>().cardType == type.Clima || eventData.pointerDrag.GetComponent<Card>().cardType == type.Despeje)
                    {
                        draggedComponent.parentToReturnTo = this.transform;
                    }      
                } 
            }
        }
}
