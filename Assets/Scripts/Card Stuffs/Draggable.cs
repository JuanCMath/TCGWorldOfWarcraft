using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform parentToReturnTo = null;
    public Transform placeholderParent = null;
    GameObject placeholder = null;
    public void OnBeginDrag(PointerEventData eventData)                                //Inicia cuando el objeto se agarra
    {
        //Debug.Log("On Being Drag");

        placeholder = new GameObject();                                                 // Creando un objeto que se quedara en el lugar de la carta agarrada
        placeholder.transform.SetParent(this.transform.parent);
        LayoutElement layele = placeholder.AddComponent<LayoutElement>();              //Agregando el componente LayoutElement a placeholder para que se comporte como una carta
        layele.preferredWidth = this.GetComponent<LayoutElement>().preferredWidth;
        layele.preferredHeight = this.GetComponent<LayoutElement>().preferredHeight;
        layele.flexibleWidth = 0;
        layele.flexibleWidth= 0;

        placeholder.transform.SetSiblingIndex(this.transform.GetSiblingIndex());       //Asignandole al placeholder el indice que tiene el objeto dentro agarrado

        parentToReturnTo = this.transform.parent;                                      // parentToReturnTo sera el parent inicial del objeto draggable
        placeholderParent = parentToReturnTo;                                          // placeholderParent estara en el parent inicial del objeto
        this.transform.SetParent(this.transform.parent.parent);                        // sacando el objeto de su parent inicial y llevandolo al abuelo o al parent de su parent

        GetComponent<CanvasGroup>().blocksRaycasts =  false;                     
    }
    public void OnDrag(PointerEventData eventData)                       //Inicia cuando el objeto esta aggarrado
    {
        this.transform.position = eventData.position;                    //Moviendo la carta a la posicion del puntero

        if (placeholder.transform.parent != placeholderParent)           // Moviendo el placeholder a donde se mueva el puntero
        {
            placeholder.transform.SetParent(placeholderParent);  
        }

        int newSiblingIndex = placeholderParent.childCount;             // Asignandole un indice dentro del Parent al placeholder

        for (int i = 0; i < placeholderParent.childCount; i++)          // 
        {
            if (this.transform.position.x < placeholderParent.GetChild(i).position.x)
            {
                newSiblingIndex = i; 

                if (placeholder.transform.GetSiblingIndex() < newSiblingIndex)
                {
                    newSiblingIndex--;
                }
                break;
            }
        }
        placeholder.transform.SetSiblingIndex(newSiblingIndex);
    }
    public void OnEndDrag(PointerEventData eventData)  //Inicia cuando el objeto es soltado
    {
        //Debug.Log("On End Drag");
        
        this.transform.SetParent(parentToReturnTo);
        this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
        GetComponent<CanvasGroup>().blocksRaycasts =  true;

        Destroy(placeholder);        
    }
}
