using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public enum Colour { red, green, blue }

    public Colour colour;
    public AnimationCurve redReaction;
    public AnimationCurve blueReaction;
    public AnimationCurve greenReaction;

    //private List<Color> colours = new List<Color> { Color.red, Color.green, Color.blue}; //change to hash map with enum as key?
    private Dictionary<Colour, Color> colours = new Dictionary<Colour, Color> { {Colour.red, Color.red}, {Colour.green, Color.green }, {Colour.blue, Color.blue} };
    private Dictionary<Colour, AnimationCurve> colourReactions;

    private Color currentColour;

    private MeshRenderer meshRenderer = null;
    private SphereCollider sphereCollider = null;
    private Rigidbody rigidbody = null;
    private List<Cell> affectingCells = new List<Cell>();

    private Camera mainCamera = null;

    private void Awake()
    {
        colourReactions = new Dictionary<Colour, AnimationCurve> { { Colour.red, redReaction }, { Colour.green, blueReaction }, { Colour.blue, greenReaction } };
        currentColour = colours[colour];

        meshRenderer = GetComponent<MeshRenderer>();
        sphereCollider = GetComponent<SphereCollider>();
        rigidbody = GetComponent<Rigidbody>();
        meshRenderer.material.color = currentColour;
        mainCamera = FindObjectOfType<Camera>();
    }

    void FixedUpdate()
    {
        //Update influences with other cells
        foreach(Cell cell in affectingCells)
        {
            Influence(cell.colour, cell.transform.position); 
            cell.Influence(this.colour, this.transform.position);
        }

        //If against a boundary, bounce
        Vector3 screenPoint = mainCamera.WorldToScreenPoint(this.transform.position);
        if (screenPoint.x <= 0 || screenPoint.x > Screen.width)
        {
            float xPos = screenPoint.x < 0 ? -mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x : mainCamera.ScreenToWorldPoint(new Vector3(Screen.width,0,0)).x;
            transform.position = new Vector3(xPos, transform.position.y, transform.position.z);
            rigidbody.velocity = new Vector3(-rigidbody.velocity.x, rigidbody.velocity.y, rigidbody.velocity.z);
            
        }
        if (screenPoint.y < 0 || screenPoint.y > Screen.height)
        {
            float yPos = screenPoint.y < 0 ? -mainCamera.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y : mainCamera.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y;
            transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, -rigidbody.velocity.y, rigidbody.velocity.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Cell>() is Cell cell)
        {
            affectingCells.Add(cell);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Cell>() is Cell cell)
        {
            if (affectingCells.Contains(cell))
            {
                affectingCells.Remove(cell);
            }
        }
    }

    private void Influence(Colour colour, Vector3 influencePosition)
    {
        Vector3 force = CalculateDirectionOfInfluence(influencePosition) * colourReactions[colour].Evaluate((Vector3.Distance(this.transform.position, influencePosition))/sphereCollider.radius);
        this.rigidbody.AddForce(force, ForceMode.Impulse);
    }

    private Vector3 CalculateDirectionOfInfluence(Vector3 influencePosition)
    {
        return (influencePosition - this.transform.position).normalized;
    }
}
