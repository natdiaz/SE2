using UnityEngine;
using System.Collections;

//definiciones que se van a usar
/*
//Tags
#define PLAYERTAG1 1
#define PLAYERTAG2 -1
//La pelota cambia de tag de acuerdo a la posision del balon

//Indexes
//Los indexes de cada pieza depende del jugador que va a jugar (pensando en nivel social)
#define PIECE_INDEX_1 1
#define PIECE_INDEX_2 2
#define PIECE_INDEX_3 3
#define PIECE_INDEX_4 4
#define PIECE_INDEX_5 5

//El index de la pelota no depende del jugador
#define BALL_INDEX 0
*/
static class Constants
{
    //Tags
	public const int PLAYERTAG1 = 1;
	public const int PLAYERTAG2 = -1;
	public const int BALLDEFAULTTAG = 0;
//La pelota cambia de tag de acuerdo a la posision del balon


//Indexes
//Los indexes de cada pieza depende del jugador que va a jugar (pensando en nivel social)
	public const int PIECE_INDEX_1 = 1;
	public const int PIECE_INDEX_2 = 2;
	public const int PIECE_INDEX_3 = 3;
	public const int PIECE_INDEX_4 = 4;
	public const int PIECE_INDEX_5 = 5;

//El index de la pelota no depende del jugador
	public const int BALL_INDEX = 6;
}
public class GameManager : MonoBehaviour {
	
	public GameObject CubeDark;
	public GameObject CubeLight;
	//public GameObjectp[] PiecesGO = new GameObjetcs[2]; 
	public GameObject[] PiecesGO = new GameObject[3]; //pelota: 0 player: 1 keeper: 2

	public int _gamLevel = 1;	//nivel 1 por default
	public int activePlayer = 1;  // 1 = White, -1 = Dark
	public int gameState = 0;			// In this state, the code is waiting for : 0 = Piece selection, 1 = Piece movement
	public int gameTurn = 0;
	public float turnTime = 0;

	public bool player1AI    = false;  // Bool that state if player1 is a AI
	public bool player2AI    = false;  // Bool that state if player2 is a AI
	public bool debugModeBool = true; //true por default para desarrollo, recuerden poner en false al entregar
	
	public Material DarkMat;
	public Material LightMat;
	public Material[] GrassMat = new Material[2];
	
	public Sprite LineasSpr;


	
	private int _boardHeight = -1;
	private int _pieceHeight =  0;
	
	private GameObject SelectedPiece;	// La pieza seleccionada

//	cambiar de boardSize a fieldSize X or Y
	//private static int _boardSize   =  8;
	private static int _fieldSizeX   =  15;
    private static int _fieldSizeY   =  11;
    /*Matriz logica
    	se accede con [x, 10-y] solo cuando se agarra las coordenadas de la matriz fisica
    	de lo contrario se accede normalmente
    */
    private int[,] _fieldPieces = new int[_fieldSizeX,_fieldSizeY];
	//private int[,] _boardPieces = new int[_boardSize,_boardSize];



	/*TODO: 
		FUNCIONES:
	 	* variable de posesion de balon
		* funcion volver a comenzar
			*pone las piezas en su lugar inicial de acuerdo al nivel

	 	* funcion posesion de balon
	 		int ballPossesion()
	 		{
	 			int i
				Vector2 _currentCoor;
				GameObject _Ball = null;
				//Debug.Log ("ballPossesion");

				_Ball = GameObject.Find("Ball");

				//guardar la posicion actual de la pelota
				_currentCoor = new Vector2(_PieceToMove.transform.position.x, _PieceToMove.transform.position.z);//z para el vector de 3D
				 
				//limitar los indices de acuerdo a la posicion del balon
				
				//recorrer el cuadrante del rededor de la posicion y sumar la cantidad de jugadores por equipo
				for(int i = 0; i < 3; i++)
				{
					for(int j = 0; j < 3; j++)
					{

					}
				}

				//sacar el max entre los dos y retornar
	 		}
			retorna: -1 si player 2 tiene mas jugadores en el cuadrante de la pelota 
				  1 si player 1 tiene mas jugadores en el cuadrante de la pelota 
				  0 si nadie tiene el balon o si hay cantidades iguales de oponentes
				  si es el arquero siempre tiene posesion de la pelota

		ACLARACIONES:
		* el gameState no cambia cuando el player tiene el balon
			el gameState no cambia si en el turno propio movio la pelota a un circulo blanco (casilla especial, se aplica regla de posesion)
			siempre que es 0 cambia de turno
		
		REGLAS DE TABLERO:
		* restringir jugadores para que no entren en el arco
			restringir la pelota para no meter en tu propia area solo si es un pase

		REGLAS DE JUEGO:
		*Portero fuera del area no tiene brazos
		
		*maximo de 4 movimientos (de jugadores, no incluye pelota??) por turno

		*existe el deathlock TODO: averiguar como es
		
		* lista de jugadas ilegales:
			*autopase
			*meter a tu area perdiendo posesion del balon
			*tirar al propio corner
			*ocupar un corner propio
			*la jugada inicial hacia atras
			*no se puede pasar la pelota por encima del arquero cuando esta en su area (el arquero en su area tiene brazos)
			*no se puede pasar la pelota por encima de los defensores dentro del area chica
	 */
	
	void Update () {
		// contar el tiempo
		turnTime += (1 * Time.deltaTime);
	}
	//GUI
	//TODO: agregar mas informacion de la partida para debug
	void OnGUI()
	{
		string _activePlayerColor;
		if(activePlayer == 1)
			_activePlayerColor = "White";
		else
			_activePlayerColor = "Dark";

		//agregar bool para debug
		GUI.Label (new Rect(10,10,200,20), ("Active Player = " + _activePlayerColor));	
		GUI.Label (new Rect(10,30,200,20), ("Game State = " + gameState));
		GUI.Label (new Rect(10,40,200,20), ("Turn Time = " + ((int)turnTime) ));


		if(debugModeBool)
		{
			GUI.Label (new Rect(10,50,200,20), ("level = " + _gamLevel));
			GUI.Label (new Rect(10,60,200,20), ("turno = " + gameTurn));
			/*TODO:datos para debbug
				datos de la partida
					*state
					*turno
					*posicion de la pieza seleccionada
					*posicion de la pelota	
			*/
			GUI.Label (new Rect(10, 70, 200, 20), ("DEBBUG MODE BOARD:"));//modificar la posision
			for(int i = 0; i < _fieldSizeX; i++)
			{
				for(int j = 0; j < _fieldSizeY; j++)
				{
					if((i==0 || i==14) && (j<3 || j>7) ) //entre 0,0 al 0,2 o 0,7 al 0,10
					{
	
					}else{
						GUI.Label (new Rect(10+15*i, (90+10*j), 200, 20), (" "+_fieldPieces[i,j]));//modificar la posision
					}
				}
			}
		}
	}


	// Initialize field
	void Start()
	{
		CreateField();
		AddBall();
		AddPieces();
	}


	// Crea la cancha
	void CreateField()
	{
	//Sprite lineasSprite;
		for(int i = 0; i < _fieldSizeX; i++)
		{
			for(int j = 0; j < _fieldSizeY; j++)
			{
				//dibujar los arcos con el mismo estilo
				if((i==0 || i==14) && (j<3 || j>7) ) //entre 0,0 al 0,2 o 0,7 al 0,10
				{										//y del 14,0 al 14,2 o 14,7 al 14,10
					//no crea el tablero

				}	
				else{	// si no crea los bloques del tablero
					if((i+j)%2 == 0)
					{
						Object.Instantiate(CubeDark,new Vector3(i,_boardHeight,j), Quaternion.identity);	
					}
					else
					{
						Object.Instantiate(CubeLight,new Vector3(i,_boardHeight,j), Quaternion.identity);
					}
				}
			}
		}
		// insertar el sprite de las lineas de la cancha
        // crear el objeto
        GameObject lineasGO = new GameObject();
        // agregar el componente "SpriteRenderer" al gameobject
        lineasGO.AddComponent<SpriteRenderer>();
        // asignar el sprite
        lineasGO.GetComponent<SpriteRenderer>().sprite = LineasSpr;
        //modificar el angulo, la position y la escala
        lineasGO.transform.Rotate(90, 0, 0);
        lineasGO.transform.position = new Vector3(7.025f,-0.486f,5.0f);
        lineasGO.transform.localScale = new Vector3(0.832f,0.832f,0.83f);

	}
	//Agrega la (unica) pelota al tablero
	void AddBall()
	{
		CreateBall(); //(x,y)
	}
	
	void CreateBall()
	{
		GameObject _PieceToCreate = null;
		int 	   _pieceIndex = Constants.BALL_INDEX;
		int _posX = 3;
		int _posY = 3;
		//Debug.Log ("CreatePosBall");
		_PieceToCreate = PiecesGO[_pieceIndex];
		// Instantiate the ball as a GameObject to be able to modify it after
		_PieceToCreate = Object.Instantiate (_PieceToCreate, new Vector3(_posX, _pieceHeight, _posY), Quaternion.identity) as GameObject; //TODO:posicion correcta en altura de la pelota
		_PieceToCreate.name = "Ball";
		
			_PieceToCreate.tag = "1";//TODO:crear el tag inicial para la pelota
			//TODO: llamar a TagBall
			_PieceToCreate.GetComponent<Renderer>().material.color = Color.yellow;

			//_PieceToCreate.GetComponent<Renderer>().material = LightMat; //TODO: ver como dejar en blando

			//agrega a la matriz tablero
		_fieldPieces[_posX,10-_posY] = 6;
		initPosBall();
	}


	//TODO: ver como se llama a la pieza para modificar su posicion al llamar a esta funcion
	void initPosBall()
	{
		int _initPosX = 7;
		int _initPosY = 5;
		Vector2 _currentCoor;
		GameObject _PieceToMove = null;
		//Debug.Log ("initPosBall");

		_PieceToMove = GameObject.Find("Ball");
		//Debug.Log ("_PieceToMove " + _PieceToMove);

		//guardar la posicion actual para cerar en el tablero
		_currentCoor = new Vector2(_PieceToMove.transform.position.x, _PieceToMove.transform.position.z);//z para el vector de 3D
		_fieldPieces[(int)_currentCoor.x,10-(int)_currentCoor.y] = 0;

		//mover la pieza y guardar en el tablero
		_PieceToMove.transform.position = new Vector3(_initPosX, _pieceHeight, _initPosY);		// Move the piece
		_fieldPieces[_initPosX,10-_initPosY] = 6;	
	}

	/*
	
	//cambia el tag del balon segun la posesion
	void PosesionBall(int _ballTag)
	{
		TODO: llamar a la funcion de posesion para saber de quien es el balon
	}

	*/

	//FUNCION MODIFICADA
	void AddPieces()
	{
		if(_gamLevel==1)
		{
			CreatePiece("Player", 4, 5, Constants.PLAYERTAG1);	
			CreatePiece("Player", 10, 5, Constants.PLAYERTAG2);
		}else if(_gamLevel==2)
		{
			CreatePiece("Player", 2, 5, Constants.PLAYERTAG1);	
		    CreatePiece("Player", 2, 6, Constants.PLAYERTAG1);	
			CreatePiece("Player", 10, 5, Constants.PLAYERTAG2);
			CreatePiece("Player", 10, 6, Constants.PLAYERTAG2);
		}else if(_gamLevel==3){
	
		}else //TODO: hacer para el level 3 con el arquero
		{
			Debug.Log ("no existe este level");
		}
	}

	//FUNCION MODIFICADA
	void CreatePiece(string _pieceName, int _posX, int _posY, int _playerTag)	
	{
		GameObject _PieceToCreate = null;
		int 	   _pieceIndex = 0;
		//index por nivel 1*_player
		if(_pieceName=="Player"){
			_pieceIndex = 1;			
		}else if(_pieceName=="Keeper"){
			_pieceIndex = 2;			
		}
		_PieceToCreate = PiecesGO[7-1];//TODO: modificar el 7

		// Instantiate the piece as a GameObject to be able to modify it after
		_PieceToCreate = Object.Instantiate (_PieceToCreate, new Vector3(_posX, _pieceHeight, _posY), Quaternion.identity) as GameObject;
		_PieceToCreate.name = _pieceName;
		if(_playerTag == 1)
		{
			_PieceToCreate.tag = "1";
			_PieceToCreate.GetComponent<Renderer>().material.color = Color.white;
		}
		else if(_playerTag == -1)
		{
			_PieceToCreate.tag = "-1";
			_PieceToCreate.GetComponent<Renderer>().material.color = Color.red;		
		}
		//Agrega a la matriz tablero
		_fieldPieces[_posX,10-_posY] = _pieceIndex*_playerTag;
	}


//TODO: funcion initPieces()
//TIP usar FindGameObjectsWithTag	Returns a list of active GameObjects tagged tag. Returns empty array if no GameObject was found.
//http://docs.unity3d.com/ScriptReference/GameObject.FindGameObjectsWithTag.html

	public void ReturnColorToPiece()
	{
		if(SelectedPiece.name == "Ball")//name pelota
		{
			Debug.Log ("es_pelota");
			SelectedPiece.GetComponent<Renderer>().material.color = Color.yellow;
		}else{
			Debug.Log ("no_es_pelota");
			if(SelectedPiece.tag == "1")//tag jugador 1
			{
				SelectedPiece.GetComponent<Renderer>().material.color = Color.white;
			}else
			{
				SelectedPiece.GetComponent<Renderer>().material.color = Color.red;
			}
		}
	}


	//FUNCION MODIFICADA
	//Update SelectedPiece with the GameObject inputted to this function
	public void SelectPiece(GameObject _PieceToSelect)
	{
		//Debug.Log ("SelectPiece");
		//TODO: solo seleccionar pelota si tenes posesion
		// Unselect the piece if it was already selected
		if(_PieceToSelect  == SelectedPiece)
		{
			//Debug.Log ("ya_seleccionada");
			ReturnColorToPiece();
			SelectedPiece = null;
			ChangeState (0);

		}
		else
		{
			//Debug.Log ("no_seleccionada tag: " + _PieceToSelect.tag);
			// Change color of the selected piece to make it apparent. Put it back to white when the piece is unselected
			if(SelectedPiece)
			{
				ReturnColorToPiece();
			}
			SelectedPiece = _PieceToSelect;
			SelectedPiece.GetComponent<Renderer>().material.color = Color.blue;
			ChangeState (1);
		}
	}


	//FUNCION MODIFICADA
	// Move the SelectedPiece to the inputted coords
	//Mueve la pieza TODO: Revisar si la pieza es pelota
	public void MovePiece(Vector2 _coordToMove)
	{
		Debug.Log ("MovePiece");
		bool validMovementBool = false;
		Vector2 _coordPiece = new Vector2(SelectedPiece.transform.position.x, SelectedPiece.transform.position.z);
		
		// Don't move if the user clicked on its own cube or if there is a piece on the cube
		//TODO: or if they ar goalkeepers arms on the cubes inside the goalkeep
		if((_coordToMove.x != _coordPiece.x || _coordToMove.y != _coordPiece.y) || _fieldPieces[(int)_coordToMove.x,10-(int)_coordToMove.y] != 0)
		{
			validMovementBool	= TestMovement (SelectedPiece, _coordToMove);
		}
		
		//Debug.Log ("MovePiece x: " + ((int)_coordToMove.x) +  " _coordToMove.y: "+ ((int)_coordToMove.y) + " _coordPiece.y: " + ((int)_coordPiece.y));

		if(validMovementBool)
		{
			_fieldPieces[(int)_coordToMove.x, 10-(int)_coordToMove.y] = _fieldPieces[(int)_coordPiece.x, 10-(int)_coordPiece.y];
			_fieldPieces[(int)_coordPiece.x , 10-(int)_coordPiece.y ] = 0;
			
			SelectedPiece.transform.position = new Vector3(_coordToMove.x, _pieceHeight, _coordToMove.y);		// Move the piece
			
			ReturnColorToPiece();// Change it's color back
			
			SelectedPiece = null;									// Unselect the Piece
			ChangeState (0);
			ChangeTurn();
		}
	}

/*
	//FUNCION ORIGINAL
	// Move the SelectedPiece to the inputted coords
	//Mueve la pieza TODO: Revisar si la pieza es pelota
	public void MovePiece(Vector2 _coordToMove)
	{
		bool validMovementBool = false;
		Vector2 _coordPiece = new Vector2(SelectedPiece.transform.position.x, SelectedPiece.transform.position.z);
		
		// Don't move if the user clicked on its own cube or if there is a piece on the cube
		//TODO: or if they ar goalkeepers arms on the cube
		if((_coordToMove.x != _coordPiece.x || _coordToMove.y != _coordPiece.y) || _boardPieces[(int)_coordToMove.x,(int)_coordToMove.y] != 0)
		{
			validMovementBool	= TestMovement (SelectedPiece, _coordToMove);
		}
		
		if(validMovementBool)
		{
			_boardPieces[(int)_coordToMove.x, (int)_coordToMove.y] = _boardPieces[(int)_coordPiece.x, (int)_coordPiece.y];
			_boardPieces[(int)_coordPiece.x , (int)_coordPiece.y ] = 0;
			
			SelectedPiece.transform.position = new Vector3(_coordToMove.x, _pieceHeight, _coordToMove.y);		// Move the piece
			SelectedPiece.GetComponent<Renderer>().material.color = Color.white;	// Change it's color back
			SelectedPiece = null;									// Unselect the Piece
			ChangeState (0);
			activePlayer = -activePlayer;
		}
	}
*/
	/*
	// If the movement is legal, eat the piece
	//ESTA FUNCION NO VAMOS A USAR
	public void EatPiece(GameObject _PieceToEat)
	{
		Vector2 _coordToEat = new Vector2(_PieceToEat.transform.position.x, _PieceToEat.transform.position.z);
		int _deltaX = (int)(_PieceToEat.transform.position.x - SelectedPiece.transform.position.x);
		
		if(TestMovement (SelectedPiece, _coordToEat) && (SelectedPiece.name != "Pawn" ||  _deltaX != 0))
		{
			Object.Destroy (_PieceToEat);
			_boardPieces[(int)_coordToEat.x, (int)_coordToEat.y] = 0; 
			MovePiece(_coordToEat);
		}
	}
	*/
	//FUNCION MODIFICADA
	//basado en TestMovement para el turno, el estado, el tag y el index
	bool TestMovement(GameObject _SelectedPiece, Vector2 _coordToMove)
	{
		bool _movementLegalBool = false;
		bool _collisionDetectBool = false;

		Vector2 _coordPiece = new Vector2(_SelectedPiece.transform.localPosition.x, _SelectedPiece.transform.localPosition.z);
		
		int _deltaX = (int)Mathf.Abs(_coordToMove.x - _coordPiece.x);
		int _deltaY = (int)Mathf.Abs(_coordToMove.y - _coordPiece.y); //con valor absoluto
		int activePlayerPawnPostion = 1;
		//Debug.Log ("_coordToMove.y " + (10-_coordToMove.y) + "_coordPiece.y " + _coordPiece.y);


		Debug.Log("Piece:(" + _coordPiece.x + "," + _coordPiece.y + ") Move:(" + _coordToMove.x + "," + ((int)( _coordToMove.y)) + ")");
		//Debug.Log("Piece (" + _coordPiece.x + "," + _coordPiece.y + ") - Move (" + _coordToMove.x + ","((int)(10 + _coordToMove.y)) + ")");
		Debug.Log("Delta (" + _deltaX + "," + _deltaY + ")");

		// Use the name of the _SelectedPiece GameObject to find the piece used
		switch (_SelectedPiece.name)
		{
			case "Ball":
				Debug.Log ("entro a Ball max: " + ((int)Mathf.Max(_deltaX, _deltaY)) + "_deltaX: " + _deltaX + "_deltaY: " + _deltaY);
				// King can only move one
				if( (int)Mathf.Max(_deltaX, _deltaY) <= 4)
				{
					if(_deltaX == _deltaY || ((_deltaX == 0) && (_deltaY != 0)) || ((_deltaX != 0) && (_deltaY == 0))) 
					{
						_movementLegalBool = true;
					}
				}
		        break;
			case "Player":
				//Debug.Log ("entro a Player max: " + ((int)Mathf.Max(_deltaX, _deltaY)) + " _deltaX: " + _deltaX + " _deltaY: " + _deltaY);
				// King can only move one
				if( (int)Mathf.Max(_deltaX, _deltaY) <= 2)
				{
					if(_deltaX == _deltaY || ((_deltaX == 0) && (_deltaY != 0)) || ((_deltaX != 0) && (_deltaY == 0))) 
					{
						//Debug.Log ("ES VALIDO!");
						_movementLegalBool = true;
					}
					//Debug.Log ("NO ES VALIDO!");
				}
		        break;
			case "Keeper":
				Debug.Log ("entro a Keeper max: " + ((int)Mathf.Max(_deltaX, _deltaY)) + "_deltaX: " + _deltaX + "_deltaY: " + _deltaY);
				// King can only move one
				if( (int)Mathf.Max(_deltaX, _deltaY) <= 2)
				{
					if(_deltaX == _deltaY || ((_deltaX == 0) && (_deltaY != 0)) || ((_deltaX != 0) && (_deltaY == 0))) 
					{
						_movementLegalBool = true;
					}
				}
				break;   
		    default:
		        _movementLegalBool = false;
		        break;
		}
		

		// If the movement is legal, detect collision with piece in the way. Don't do it with the ball since they can pass over pieces TODO: unless is a goalkeeper in his area.
		//Si el movimiento es legal, detectar colision con otra pieza
		if(_movementLegalBool && SelectedPiece.name != "Pelota")
		{
			_collisionDetectBool = TestCollision (_coordPiece, _coordToMove);
		}
			
		return (_movementLegalBool && !_collisionDetectBool);
	}


	/*
	//FUNCION ORIGINAL
	// Test if the piece can do the player's movement
	bool TestMovement(GameObject _SelectedPiece, Vector2 _coordToMove)
	{
		bool _movementLegalBool = false;
		bool _collisionDetectBool = false;

		Vector2 _coordPiece = new Vector2(_SelectedPiece.transform.localPosition.x, _SelectedPiece.transform.localPosition.z);
		
		int _deltaX = (int)(_coordToMove.x - _coordPiece.x);
		int _deltaY = (int)(_coordToMove.y - _coordPiece.y);
		int activePlayerPawnPostion = 1;

		//Debug.Log("Piece (" + _coordPiece.x + "," + _coordPiece.x + ") - Move (" + _coordToMove.x + "," + _coordToMove.y + ")");
		//Debug.Log("Delta (" + _deltaX + "," + _deltaY + ")");

		// Use the name of the _SelectedPiece GameObject to find the piece used
		switch (_SelectedPiece.name)
		{
			
		    case "Pawn": 
				if(activePlayer == -1)
					activePlayerPawnPostion = 6;		
			
			
				// Pawn can move 1 steap ahead of them, 2 if they are on their initial position
		        if(_deltaY == activePlayer || (_deltaY == 2*activePlayer && _coordPiece.y == activePlayerPawnPostion))
				{
				//Debug.Log ("_deltaY");
					_movementLegalBool = true;
				}
				
		        break;
		    case "Rook":
				// Rook can move horizontally or vertically
				if((_deltaX != 0 && _deltaY == 0) || (_deltaX == 0 && _deltaY != 0)) 
				{
					_movementLegalBool = true;
				}
		        break;
			case "Knight":
				// Knight move in a L movement. distance is evaluated by a multiplication of both direction
				if((_deltaX != 0 && _deltaY != 0) && Mathf.Abs(_deltaX*_deltaY) == 2)
				{
					_movementLegalBool = true;
				}
		        break;
			case "Bishop":
				// Bishop can only move diagonally
				if(Mathf.Abs(_deltaX/_deltaY) == 1)
				{
					_movementLegalBool = true;
				}
		        break;
			
			case "Queen":
				// Queen movement is a combination of Rook and bishop
				if(((_deltaX != 0 && _deltaY == 0) || (_deltaX == 0 && _deltaY != 0)) || Mathf.Abs(_deltaX/_deltaY) == 1)
				{
					_movementLegalBool = true;
				}
		        break;
			
			case "King":
				// King can only move one
				if(Mathf.Abs(_deltaX + _deltaY) == 1) 
				{
					_movementLegalBool = true;
				}
		        break;
			
		    default:
		        _movementLegalBool = false;
		        break;
		}
		
		// If the movement is legal, detect collision with piece in the way. Don't do it with knight since they can pass over pieces.
		if(_movementLegalBool && SelectedPiece.name != "Knight")
		{
			_collisionDetectBool = TestCollision (_coordPiece, _coordToMove);
		}
			
		return (_movementLegalBool && !_collisionDetectBool);
	}
	

	*/
	//FUNCION MODIFICADA
	//TODO: la pelota no puede atravezar ni el arquero ni sus brazos dentro del area
	// Test if a unit is in the path of the tested movement
	bool TestCollision(Vector2 _coordInitial, Vector2 _coordFinal)
	{
		bool CollisionBool = false;
		//los delta se dejan sin valores absolutos
		int _deltaX = (int)(_coordFinal.x - _coordInitial.x);
		int _deltaY = (int)((_coordFinal.y) - _coordInitial.y); 
		int _incX = 0; // Direccion del incremento en X
		int _incY = 0; // Direccion del incremento en X
		int i;
		int j;
		//Debug.Log ("TestCollision");

		// Cacula el incremento (que puede ser negativo) evita la division por 0
		if(_deltaX != 0)
		{
			_incX = (_deltaX/Mathf.Abs(_deltaX));
		}
		if(_deltaY != 0)
		{
			_incY = (_deltaY/Mathf.Abs(_deltaY));
		}
		
		//Debug.Log ("_coordInit:(" + ((int)_coordInitial.x) + "," + ((int)_coordInitial.y) + ") _coordFinal:(" + ((int)(_coordFinal.x)) + "," + ((int)(_coordFinal.y)) + ")");

		i = (int)_coordInitial.x + _incX;
		j = (int)_coordInitial.y + _incY;
		
		while(new Vector2(i, j) != _coordFinal)
		{
			if(_fieldPieces[i,10-j] != 0)
			{
				CollisionBool = true;
				break;
			}
			i += _incX;
			j += _incY;
		}
		return CollisionBool;
	}

	// Change the state of the game from Piece selection to Piece movement and viceversa
	public void ChangeTurn()
	{
			activePlayer = -activePlayer;
			turnTime = 0;
			gameTurn++;
	}


	// Change the state of the game from Piece selection to Piece movement and viceversa
	public void ChangeState(int _newState)
	{
		gameState = _newState;
	}
}
