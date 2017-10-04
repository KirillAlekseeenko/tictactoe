using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
		var nextMove = minimax (field, Turn.AI, null);
		i = nextMove.pair.x;
		j = nextMove.pair.y;
	} 

	private int score(State state)
	{
		if (state == State.PlayerWin)
			return -10;
		else if (state == State.AIWin)
			return 10;
		else
			return 0;
	}

	private Dictionary<Pair, Cell> unfoldMap(Cell[,] board)
	{
		Dictionary<Pair, Cell> map = new Dictionary<Pair, Cell> ();
		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < columns; j++) {
				map.Add (new Pair (i, j), board[i, j]);
			}
		}
		return map;
	}

	private bool decision(Dictionary<int, Cell> board, Cell player)
	{
		if (
			(board [0] == player && board [1] == player && board [2] == player) ||
			(board [3] == player && board [4] == player && board [5] == player) ||
			(board [6] == player && board [7] == player && board [8] == player) ||
			(board [0] == player && board [3] == player && board [6] == player) ||
			(board [1] == player && board [4] == player && board [7] == player) ||
			(board [2] == player && board [5] == player && board [8] == player) ||
			(board [0] == player && board [4] == player && board [8] == player) ||
			(board [2] == player && board [4] == player && board [6] == player))
			return true;
		else
			return false;
	}


	private Move minimax(Cell[,] board, Turn player, Pair lastPair)
	{
		var moves = new List<Move>();
		var map = unfoldMap (board);

		int res = score(
			getState(
				decision (map.ToDictionary (p => p.Key.unfold (), p => p.Value), makeMoveByTurn (player)),
				player)
		);

		if (lastPair != null) {
			if (res != 0)
				return new Move (lastPair, res);
			if (!map.Values.Contains (Cell.None))
				return new Move(lastPair, 0);
		}


		var availableMoves = map.Where (m => m.Value == Cell.None).ToDictionary (m => m.Key, m => m.Value).Keys.ToList();

		foreach (Pair pair in availableMoves) {
			
			Move move;

			var updatedBoard = board;
			updatedBoard [pair.x, pair.y] = makeMoveByTurn (player);
			if (player == Turn.AI) {
				move = minimax (updatedBoard, Turn.Player, pair);
			} else {
				move = minimax (updatedBoard, Turn.AI, pair);
			}

			moves.Add (move);
		}

		Move bestMove;

		if (player == Turn.AI) {
			var maxScore = moves.Max (m => m.score);
			bestMove = moves.Where (m => m.score == maxScore).First ();
		} else {
			var minScore = moves.Min (m => m.score);
			bestMove = moves.Where (m => m.score == minScore).First ();
		}

		return bestMove;
	}

	private Cell makeMoveByTurn(Turn turn)
	{
		if (turn == Turn.AI)
			return Cell.Black;
		else
			return Cell.White;
	}

	private State getState(bool decision, Turn turn)
	{
		if (decision && turn == Turn.AI)
			return State.AIWin;
		else if (decision && turn == Turn.Player)
			return State.PlayerWin;
		else
			return State.InProgress;
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

class Pair{
	public int x;//i
	public int y;//j
	public Pair(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public int unfold(){
		return x * 3 + y;
	}
}

class Move
{
	public Pair pair;
	public int score;
	public Move(Pair pair, int score)
	{
		this.pair = pair;
		this.score = score;
	}
}

enum Cell {White, Black, None};

enum Turn{Player, AI};

enum State{PlayerWin, AIWin, InProgress}
