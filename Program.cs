using System.Text;
namespace ObjectivelyChess;

public enum Piece
{
    wKing = 0,
    wQueen = 1,
    wBishop = 3,
    wKnight = 4,
    wRook = 2,
    wPawn = 5,
    bKing = 6,
    bQueen = 7,
    bBishop = 9,
    bKnight = 10,
    bRook = 8,
    bPawn = 11,
    empty = 12
}
public class Utilities
{
    public static readonly char[] whitePieceArray = { '♔', '♕', '♖', '♗', '♘', '♙' };
    public static readonly char[] blackPieceArray = { '♚', '♛', '♜', '♝', '♞', '♟' };
    public static readonly char[] pieceArray = { '♔', '♕', '♖', '♗', '♘', '♙', '♚', '♛', '♜', '♝', '♞', '♟', '.' }; // TODO: Replace . with black and white squares.
    public static int instantiatedPieces = 0;
    public static char GetPieceChar(Piece piece)
    {
        return pieceArray[(int)piece];
    }
    public static bool IsWhite(int[] location)
    {
        if (Board.boardArr == null) throw new Exception("Board null at IsWhite");
        if ((int)Board.boardArr[location[0], location[1]] <= 5)
            return true;
        return false;
    }
    public static bool IsWhite(Piece piece)
    {
        if ((int)piece <= 5)
            return true;
        return false;
    }
    public static bool IsEmpty(int[] location)
    {
        if (Board.boardArr == null) throw new Exception("Board null at IsEmpty");
        if ((int)Board.boardArr[location[0], location[1]] == 12)
            return true;
        return false;
    }
    public static bool SetPieceToDead(int[] location) // True for successful
    {
        if (Board.SpecPieceArray[location[0], location[1]] == null) throw new Exception("SpecPieceArray null at SetPieceToDead");
        return Board.SpecPieceArray[location[0], location[1]]!.Kill();
    }
}
class Square
{
    private Piece _occupant = Piece.empty;
    public bool IsEmpty()
    {
        return (_occupant == Piece.empty);
    }
    public Piece GetPiece()
    {
        return _occupant;
    }
}
class Board
{
    private const int _width = 8;
    private const int _height = 8;
    public static Piece[,]? boardArr;
    private readonly Piece[,] initialBoard = { // From H1 to A8
        { Piece.bRook, Piece.bKnight, Piece.bBishop, Piece.bQueen, Piece.bKing, Piece.bBishop, Piece.bKnight, Piece.bRook}, // NTS: Find VIM motion to do this faster & Fix formatting for arrays
        { Piece.bPawn, Piece.bPawn ,Piece.bPawn, Piece.bPawn, Piece.bPawn, Piece.bPawn, Piece.bPawn, Piece.bPawn},
        { Piece.empty, Piece.empty, Piece.empty, Piece.empty, Piece.empty, Piece.empty, Piece.empty, Piece.empty,},
        { Piece.empty, Piece.empty, Piece.empty, Piece.empty, Piece.empty, Piece.empty, Piece.empty, Piece.empty,},
        { Piece.empty, Piece.empty, Piece.empty, Piece.empty, Piece.empty, Piece.empty, Piece.empty, Piece.empty,},
        { Piece.empty, Piece.empty, Piece.empty, Piece.empty, Piece.empty, Piece.empty, Piece.empty, Piece.empty,},
        { Piece.empty, Piece.empty, Piece.empty, Piece.empty, Piece.empty, Piece.empty, Piece.empty, Piece.empty,},
        { Piece.wPawn, Piece.wPawn ,Piece.wPawn, Piece.wPawn, Piece.wPawn, Piece.wPawn, Piece.wPawn, Piece.wPawn},
        { Piece.wRook, Piece.wKnight, Piece.wBishop, Piece.wQueen, Piece.wKing, Piece.wBishop, Piece.wKnight, Piece.wRook},
    };
    public static SpecPiece?[,] SpecPieceArray = new SpecPiece?[_width, _height];
    public void Initialise()
    {
        boardArr = initialBoard;
    }
    public void DisplayBoard(char[,] buffer)
    {
        ConsoleRenderer.clearBuffer(buffer);
        int offX = 2;
        int offY = 2;

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                if (boardArr == null) throw new Exception("Board null at DisplayBoard");
                Piece pieceItem = boardArr[y, x];
                char piece = Utilities.GetPieceChar(pieceItem);
                bool isDark = ((x + y) % 2 == 1);
                char squareChar = isDark ? '#' : '.';
                if (piece == '.')
                {
                    ConsoleRenderer.DrawChar(buffer, offX + x, offY + y, squareChar);
                }
                else
                {
                    ConsoleRenderer.DrawChar(buffer, offX + x, offY + y, piece);
                }
            }
        }
    }
    public Piece GetSquare(int x, int y)
    {
        if (boardArr == null) throw new Exception("Board null at GetSquare");
        return boardArr[x, y];
    }
    public bool IsSquareOccupied(int x, int y)
    {
        if (boardArr == null) throw new Exception("Board null at IsSquareOccupied");
        return (Utilities.GetPieceChar(boardArr[x, y]) == '.');
    }
}
abstract class SpecPiece
{
    private Piece _pieceType;
    private int _pieceID;
    private bool _isAlive;
    private int[] _currentSquare = { -1, -1 };
    public void InstantiatePiece()
    {
        _pieceID = Utilities.instantiatedPieces;
        Utilities.instantiatedPieces++;
        Board.SpecPieceArray[_currentSquare[0], _currentSquare[1]] = this;
    }
    public bool Kill()
    {
        if (!_isAlive)
        {
            _isAlive = false;
            Board.SpecPieceArray[_currentSquare[0], _currentSquare[1]] = null;
            return true;
        }
        return false;
    }
    abstract public bool isValidMove(int xDiff, int yDiff);
    public bool TryMove(int xDiff, int yDiff) // true for successful
    {
        bool returnValue = false;
        if (isValidMove(xDiff, yDiff))
        {
            _currentSquare[0] = _currentSquare[0] + xDiff;
            _currentSquare[1] = _currentSquare[0] + yDiff;
            returnValue = true;
        }
        bool OpponentIsWhite = !(Utilities.IsWhite(_pieceType));
        if (Board.boardArr == null) throw new Exception("Board null at IsSquareOccupied");
        if (Utilities.IsWhite(Board.boardArr[_currentSquare[0], _currentSquare[1]]) == OpponentIsWhite && Utilities.IsEmpty(_currentSquare) == false)
        {
            // But how to make other piece dead?
            // Using an enum was so smart until it wasn't.
            // Do I even need this abstract piece class? Can't I just handle each piece individually with one big function?
            // Actually this is in the spec so I don't really have a choice, do I...
            // I'm thinking...
            // The order this was written in the spec is so weird. You should make the piece classes BEFORE the board so that you can initialise them and use them in the board array!
            // And I still haven't done all the movement logic yet.
            // Let alone the gameplay loop.
            // The music is very fitting though.
            // I'm thinking...
            // Oh I know, an instantiated piece finder helper!
            // I won't explain, I'll just show...
            // It didn't like it, I'll try something else...
            // I'm going to add this to git quickly so that I can do commits and get my graph up again
            // This would work but I can't make it put itself in the SpecPiece array because IT DOESN'T LIKE IT!
            // Maybe just put the ID?
            // But then I get the same issue as before of having to search through the IDs, and that won't work unless I make a bunch of changes.
            // I'm thinking...
            // OMG I can I just put 'this'.
            // Alright, now what?
            // I've forgotten I've been gone for around 4 minutes.
            // Oh right, the spec.
            // Oh wait, I was writing this function, wasn't I!
            // I need to make it take pieces now.
            // Which I still need to figure out.
            // This large block of comments will be staying in the code, I didn't spend all this time writing to myself for nothing! Now everyone can see how crazy I've gone doing this so late.
            // Why am I even adding these 'True for successful' comments? It's not like I'm going to use them ever. Even for debugging I'll just log it...
            // I should've made the stuff in Board take a coordinate as ain int[] as well as just x and y - I was going to do that originally but I didn't and it would be really nice right now.
            // It's fine I can deal with it.
            // Okay, I'm at the if statement of this block, on the and half, and it's complaining thet Utilities.IsEmpty() doesn't returna bool? Huh?
            // That one tiny commit took 20 minutes -_-.
            // I'm really slowing down. It's only 22:19, but I guess I've done a lot of other work today.
            // That commit added 9 lines and removed 4 D:.
            // Also, I must keep the proper punctuation consistent, even with emojis!
            // Three of those 9 lines were comments here...
            // OMG I'm just realising how much I have left, there's no way I can complete this tonight and finish all of my other work!
            // It's only 213 lines too (214 now I'm writing this). My brain is just so fried tonight.
            // Okay, what next I guess...
            // Specific piece class time! Wow, this is the first time I'm defining pieces because I thought ahead, it's not like I already made an enum and used that for everything previously!
            // It's not like I did that at all no, it would require an entire rewrite to make it work with these classes! That would be silly!
            // But the music is getting mor exciting again, maybe it's comeback time...
            // I'm using the mouse by accident again...
            // Also, I need to figure out how to resize each window in a tab split.
            // I was going to start with the king, but actually checking for a valid move is really hard.
            // Wait, it's really hard for everything because it needs to take the king out of check!
            // But that's game logic stuff!
            // I'll just make a public bool at the top of the program and call it a probelm for later me.
            // Alright, new side of record, new me- I mean a fresh start- I mean... Yeah this doesn't change anything. But the music is much more active, maybe it'll hype me up.
            // This is so weird just commenting in the comments. But it's the only break I have from programming so...
            // I forgot how to override an abstract function for a second... what am I doingggggg!
            // Ok placeholder function for checking in check, let's gooooooo...
            // I wonder if anyone else has actually completed this task.
            // Let me procrastinate by asking someone...
            // Ok the bop remix is on, subwoofer time!
            // It's called 'Celestial Resort (Good Karma Mix)' I think.
            // Ok, do I have separate classes for the white and black pieces...?
            // Oh that's cool, my nvim config highlights colours. It looks odd in the comments though.
            // Ok I'm going to stop screen recording in a second and maybe take a break for a bit. I won't use up more storage on this silly screen recording...
            // Bye, whoever's reading this.
            // Bye, me.
            Utilities.SetPieceToDead(_currentSquare);
        }
        return returnValue;
    }
}
//class King : SpecPiece
//{
//    private override Piece _pieceType = Piece.wKing; 
//    public override bool isValidMove(int xDiff, int yDiff) {
//        if ()
//    }
//}
class Game
{
    public bool whiteInCheck = false;
    public bool blackInCheck = false;

}
class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        ConsoleRenderer.Init();
        var buffer = ConsoleRenderer.CreateBuffer();
        Console.WriteLine("Hello, World!");
    }
}

