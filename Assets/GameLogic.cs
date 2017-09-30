using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameLogic : MonoBehaviour {


	public List<Button> buttonField;
	public Sprite whiteCircle;
	public Sprite blackCircle;


	private Cell[,] field;
	private Turn currentTurn;

	private const int rows = 3;
	private const int columns = 3;

	private int unfold(int i, int j) // converts field[i,j] into buttonField[index]
	{
		return i * rows + j;
	}
	private void initializeField()
	{
		field = new Cell[rows, columns];
		for (int i = 0; i < rows; i++)
			for (int j = 0; j < columns; j++) {
				field [i, j] = Cell.None;
				int index = unfold (i, j);
				buttonField [index].onClick.AddListener (() => 
					{
						if(currentTurn == Turn.Player)
						{
							buttonField[index].GetComponent<Image>().sprite = whiteCircle;
							field [i, j] = Cell.White;
							currentTurn = Turn.AI;
						}
					});
			}

	}



	private void AI(out int i, out int j) // i,j - place, where AI decided to instantiate circle or cross
	{
		
	}


	// Use this for initialization
	void Start () {

		initializeField ();
	
	}
	
	// Update is called once per frame
	void Update () {

		if (currentTurn == Turn.AI) {
			int i, j;
			AI (out i, out j);

			buttonField [unfold (i, j)].GetComponent<Image> ().sprite = blackCircle;
			field [i, j] = Cell.Black;
			currentTurn = Turn.Player;
		}
		
	}


}

enum Cell {White, Black, None};

enum Turn{Player, AI};
