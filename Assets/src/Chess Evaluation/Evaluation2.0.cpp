// Base researched criteria (Material, King Safety, Space, Mobility (could be similar to space), Piece Coordination, Pawn structure)
// Personal additions: Forced combination advantage (search will do this), Piece positioning, passed pawns
// Will use principle evaluation ideas + extensions/customizations but with unique methods and weights
// https://chatgpt.com/ used for idea generation for optimizations
// Credits to https://github.com/nkarve/surge for fast bitboard legal chess move generator and zobrist hashing (Stockfish Wrapper)
// Used Stockfish Engine 17 Open Source Chess Engine for efficient UCI and benchmarking https://stockfishchess.org/
// Source: https://chessify.me/blog/chess-engine-evaluation
// Check pieces around the king
// Alpha beta pruning + minimax aglorithm https://www.youtube.com/watch?v=l-hh51ncgDI
// For board scores

#include <iostream>
#include <fstream>
#include <vector>
#include <sstream>
#include <cmath>
#include <string>
#include <algorithm>
#include <unordered_map>
#include <tuple>
#include <iomanip>

#include "./surge/src/position.h"
#include "./surge/src/tables.h"
#include "./surge/src/types.h"

using namespace std;

bool whiteTurn = true; // Is it white or black's turn

// Define a pair for evaluation and best move
using EvalMoveTuple = tuple<float, Move>;

unordered_map<uint64_t, EvalMoveTuple> transpositionTable;

void EvaluationSequence();
EvalMoveTuple alphabetaPrunePositions(Position& p, int depth, float alpha, float beta);
float evaluatePos(Position& position);
float pieceScore(Position& position);
float kingScore(Position& position);
float mobilityScore(Position& position);
float coordScore(Position& position);
float pawnStructureScore(Position& position);

int main() {
    EvaluationSequence();
    return 0;
}

void EvaluationSequence() {
    initialise_all_databases();
    zobrist::initialise_zobrist_keys();

    Position p;
    p.set("3K4/Pp1p1N1n/bpPppk1P/PN2pp2/1p1P1PBb/2RPB2q/3Pn3/1r1Q2Rr b - - 0 1", p);

    whiteTurn = p.turn() == WHITE;

    int depth = 6;

    EvalMoveTuple bestMoveResult = alphabetaPrunePositions(p, depth, -100000, 100000);

    cout << fixed << setprecision(2);
    cout << "Board: \n" << p << endl;
    cout << "Best score: " << get<0>(bestMoveResult) << endl;
    cout << "Best move: " << get<1>(bestMoveResult) << endl;

    cout << "Total positions at depth " << depth << ": " << transpositionTable.size() << std::endl;
}

EvalMoveTuple alphabetaPrunePositions(Position& p, int depth, float alpha, float beta) {

    if (depth == 0) {
        return { evaluatePos(p), Move() };  // Return evaluation and an empty move
    }

    uint64_t zKey = p.get_hash();

    if (transpositionTable.find(zKey) != transpositionTable.end()) {
        return transpositionTable[zKey];
    }

    Move bestMove;
    float extEval = (p.turn() == WHITE) ? -100000 : 100000;

    if (p.turn() == WHITE) { // Max Player
        MoveList<WHITE> list(p);
        for (Move m : list) {
            p.play<WHITE>(m);
            EvalMoveTuple result = alphabetaPrunePositions(p, depth - 1, alpha, beta);
            float eval = get<0>(result);
            if (eval > extEval) {
                extEval = eval;
                bestMove = m;
            }
            alpha = max(alpha, extEval);
            p.undo<WHITE>(m);
            if (beta <= alpha) break;
        }
    }
    else { // Min Player
        MoveList<BLACK> list(p);
        for (Move m : list) {
            p.play<BLACK>(m);
            EvalMoveTuple result = alphabetaPrunePositions(p, depth - 1, alpha, beta);
            float eval = get<0>(result);
            if (eval < extEval) {
                extEval = eval;
                bestMove = m;
            }
            beta = min(beta, extEval);
            p.undo<BLACK>(m);
            if (beta <= alpha) break;
        }
    }

    transpositionTable[zKey] = { extEval, bestMove };  // Store evaluation and move
    return transpositionTable[zKey];
}


float evaluatePos(Position& position) {

    float position_score = 0;

    float piece_score = pieceScore(position);
    float king_safety_score = kingScore(position);
    float mobility_score = mobilityScore(position);
    float coord_score = coordScore(position);
    float pawn_structure_score = pawnStructureScore(position);

    position_score = piece_score + king_safety_score + mobility_score + coord_score + pawn_structure_score;

    return position_score;

}

float pieceScore(Position& position) {

    float score = 0;
    string positionFen = position.fen();
    string boardFen = positionFen.substr(0, positionFen.find(' '));

    // https://www.freecodecamp.org/news/simple-chess-ai-step-by-step-1d55a9266977 Source for these values

    vector<vector<float>> preferred_white_pawn_squares = {
        { 0.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0},
        { 5.0,  5.0,  5.0,  5.0,  5.0,  5.0,  5.0,  5.0},
        { 1.0,  1.0,  2.0,  3.0,  3.0,  2.0,  1.0,  1.0},
        { 0.5,  0.5,  1.0,  2.5,  2.5,  1.0,  0.5,  0.5},
        { 0.0,  0.0,  0.0,  2.0,  2.0,  0.0,  0.0,  0.0},
        { 0.5, -0.5, -1.0,  0.0,  0.0, -1.0, -0.5,  0.5},
        { 0.5,  1.0,  1.0, -2.0, -2.0,  1.0,  1.0,  0.5},
        { 0.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0}
    };

    vector<vector<float>> preferred_black_pawn_squares = {
        { 0.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0},
        {-0.5, -1.0, -1.0,  2.0,  2.0,-1.0,  -1.0, -0.5},
        {-0.5,  0.5,  1.0,  0.0,  0.0,  1.0,  0.5, -0.5},
        { 0.0,  0.0,  0.0, -2.0, -2.0,  0.0,  0.0,  0.0},
        {-0.5, -0.5, -1.0, -2.5, -2.5, -1.0, -0.5, -0.5},
        {-1.0, -1.0, -2.0, -3.0, -3.0, -2.0, -1.0, -1.0},
        {-5.0, -5.0, -5.0, -5.0, -5.0, -5.0, -5.0, -5.0},
        { 0.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0}
    };



    vector<vector<float>> preferred_white_bishop_squares = {
        {-2.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -2.0},
        {-1.0,  1.0,  0.0,  0.0,  0.0,  0.0,  1.0, -1.0},
        {-1.0,  1.0,  1.0,  1.0,  1.0,  1.0,  1.0, -1.0},
        {-1.0,  0.0,  1.0,  1.0,  1.0,  1.0,  0.0, -1.0},
        {-1.0,  0.5,  0.5,  1.0,  1.0,  0.5,  0.5, -1.0},
        {-1.0,  0.0,  0.5,  1.0,  1.0,  0.5,  0.0, -1.0},
        {-1.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0, -1.0},
        {-2.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -2.0}
    };

    vector<vector<float>> preferred_black_bishop_squares = {
        { 2.0,  1.0,  1.0,  1.0,  1.0,  1.0,  1.0,  2.0},
        { 1.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0,  1.0},
        { 1.0,  0.0, -0.5, -1.0, -1.0, -0.5,  0.0,  1.0},
        { 1.0, -0.5, -0.5, -1.0, -1.0, -0.5, -0.5,  1.0},
        { 1.0,  0.0, -1.0, -1.0, -1.0, -1.0,  0.0,  1.0},
        { 1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,  1.0},
        { 1.0, -1.0,  0.0,  0.0,  0.0,  0.0, -1.0,  1.0},
        { 2.0,  1.0,  1.0,  1.0,  1.0,  1.0,  1.0,  2.0}
    };

    vector<vector<float>> preferred_white_knight_squares = {
        {-5.0, -4.0, -3.0, -3.0, -3.0, -3.0, -4.0, -5.0},
        {-4.0, -2.0,  0.0,  0.0,  0.0,  0.0, -2.0, -4.0},
        {-3.0,  0.0,  1.0,  1.5,  1.5,  1.0,  0.0, -3.0},
        {-3.0,  0.5,  1.5,  2.0,  2.0,  1.5,  0.5, -3.0},
        {-3.0,  0.0,  1.5,  2.0,  2.0,  1.5,  0.0, -3.0},
        {-3.0,  0.5,  1.0,  1.5,  1.5,  1.0,  0.5, -3.0},
        {-4.0, -2.0,  0.0,  0.5,  0.5,  0.0, -2.0, -4.0},
        {-5.0, -4.0, -3.0, -3.0, -3.0, -3.0, -4.0, -5.0}
    };

    vector<vector<float>> preferred_black_knight_squares = {
        {5.0, 4.0, 3.0, 3.0, 3.0, 3.0, 4.0, 5.0},
        {4.0, 2.0, 0.0, 0.0, 0.0, 0.0, 2.0, 4.0},
        {3.0, 0.0,-1.0,-1.5,-1.5,-1.0, 0.0, 3.0},
        {3.0,-0.5,-1.5,-2.0,-2.0,-1.5,-0.5, 3.0},
        {3.0, 0.0,-1.5,-2.0,-2.0,-1.5, 0.0, 3.0},
        {3.0,-0.5,-1.0,-1.5,-1.5,-1.0,-0.5, 3.0},
        {4.0, 2.0, 0.0,-0.5,-0.5, 0.0, 2.0, 4.0},
        {5.0, 4.0, 3.0, 3.0, 3.0, 3.0, 4.0, 5.0}
    };

    vector<vector<float>> preferred_white_rook_squares = {
        { 0.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0},
        { 0.5,  1.0,  1.0,  1.0,  1.0,  1.0,  1.0,  0.5},
        {-0.5,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0, -0.5},
        {-0.5,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0, -0.5},
        {-0.5,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0, -0.5},
        {-0.5,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0, -0.5},
        {-0.5,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0, -0.5},
        { 0.0,  0.0,  0.0,  0.5,  0.5,  0.0,  0.0,  0.0}
    };

    vector<vector<float>> preferred_black_rook_squares = {
        { 0.0,  0.0,  0.0, -0.5, -0.5,  0.0,  0.0,  0.0},
        { 0.5,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.5},
        { 0.5,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.5},
        { 0.5,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.5},
        { 0.5,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.5},
        { 0.5,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.5},
        {-0.5, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -0.5},
        { 0.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0}
    };

    vector<vector<float>> preferred_white_queen_squares = {
        {-2.0, -1.0, -1.0, -0.5, -0.5, -1.0, -1.0, -2.0},
        {-1.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0, -1.0},
        {-1.0,  0.0,  0.5,  0.5,  0.5,  0.5,  0.0, -1.0},
        {-0.5,  0.0,  0.5,  0.5,  0.5,  0.5,  0.0, -0.5},
        { 0.0,  0.0,  0.5,  0.5,  0.5,  0.5,  0.0, -0.5},
        {-1.0,  0.5,  0.5,  0.5,  0.5,  0.5,  0.0, -1.0},
        {-1.0,  0.0,  0.5,  0.0,  0.0,  0.0,  0.0, -1.0},
        {-2.0, -1.0, -1.0, -0.5, -0.5, -1.0, -1.0, -2.0}
    };

    vector<vector<float>> preferred_black_queen_squares = {
        { 2.0,  1.0,  1.0,  0.5,  0.5,  1.0,  1.0,  2.0},
        { 1.0,  0.0, -0.5,  0.0,  0.0,  0.0,  0.0,  1.0},
        { 1.0, -0.5, -0.5, -0.5, -0.5, -0.5,  0.0,  1.0},
        { 0.0,  0.0, -0.5, -0.5, -0.5, -0.5,  0.0,  0.5},
        { 0.5,  0.0, -0.5, -0.5, -0.5, -0.5,  0.0,  0.5},
        { 1.0,  0.0, -0.5, -0.5, -0.5, -0.5,  0.0,  1.0},
        { 1.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0,  1.0},
        { 2.0,  1.0,  1.0,  0.5,  0.5,  1.0,  1.0,  2.0}
    };

    vector<vector<float>> preferred_white_king_squares = {
        { 2.0,  3.0,  1.0,  0.0,  0.0,  1.0,  3.0,  2.0},
        { 2.0,  2.0,  0.0,  0.0,  0.0,  0.0,  2.0,  2.0},
        {-1.0, -2.0, -2.0, -2.0, -2.0, -2.0, -2.0, -1.0},
        {-2.0, -3.0, -3.0, -1.0, -1.0, -3.0, -3.0, -2.0},
        {-3.0, -4.0, -4.0, -5.0, -5.0, -4.0, -4.0, -3.0},
        {-3.0, -4.0, -4.0, -5.0, -5.0, -4.0, -4.0, -3.0},
        {-3.0, -4.0, -4.0, -5.0, -5.0, -4.0, -4.0, -3.0},
        {-3.0, -4.0, -4.0, -5.0, -5.0, -4.0, -4.0, -3.0}
    };

    vector<vector<float>> preferred_black_king_squares = {
        { 3.0,  4.0,  4.0,  5.0,  5.0,  4.0,  4.0,  3.0},
        { 3.0,  4.0,  4.0,  5.0,  5.0,  4.0,  4.0,  3.0},
        { 3.0,  4.0,  4.0,  5.0,  5.0,  4.0,  4.0,  3.0},
        { 3.0,  4.0,  4.0,  5.0,  5.0,  4.0,  4.0,  3.0},
        { 2.0,  3.0,  3.0,  4.0,  4.0,  3.0,  3.0,  2.0},
        { 1.0,  2.0,  2.0,  2.0,  2.0,  2.0,  2.0,  1.0},
        {-2.0, -3.0, -1.0,  0.0,  0.0, -1.0, -3.0, -2.0},
        {-2.0, -2.0,  0.0,  0.0,  0.0,  0.0, -2.0, -2.0}
    };

    int rank = 7, file = 0;
    int adjFactorPos = 1;

    for (char piece : boardFen) {
        if (isdigit(piece)) {
            file += piece - '0';
        }
        else if (piece == '/') {
            rank--;
            file = 0;
        }
        else {
            if (piece == 'P') {
                score = score + 1;
                score = score + preferred_white_pawn_squares[rank][file] * adjFactorPos;
            }
            else if (piece == 'p') {
                score = score - 1;
                score = score + preferred_black_pawn_squares[rank][file] * adjFactorPos;
            }
            else if (piece == 'K') {
                score = score + 1000;
                score = score + preferred_white_king_squares[rank][file] * adjFactorPos;
            }
            else if (piece == 'k') {
                score = score - 1000;
                score = score + preferred_black_king_squares[rank][file] * adjFactorPos;
            }
            else if (piece == 'B') {
                score = score + 3.15;
                score = score + preferred_white_bishop_squares[rank][file] * adjFactorPos;
            }
            else if (piece == 'b') {
                score = score - 3.15;
                score = score + preferred_black_bishop_squares[rank][file] * adjFactorPos;
            }
            else if (piece == 'N') {
                score = score + 3;
                score = score + preferred_white_knight_squares[rank][file] * adjFactorPos;
            }
            else if (piece == 'n') {
                score = score - 3;
                score = score + preferred_black_knight_squares[rank][file] * adjFactorPos;
            }
            else if (piece == 'R') {
                score = score + 5;
                score = score + preferred_white_rook_squares[rank][file] * adjFactorPos;
            }
            else if (piece == 'r') {
                score = score - 5;
                score = score + preferred_black_rook_squares[rank][file] * adjFactorPos;
            }
            else if (piece == 'Q') {
                score = score + 9;
                score = score + preferred_white_queen_squares[rank][file] * adjFactorPos;
            }
            else if (piece == 'q') {
                score = score - 9;
                score = score + preferred_black_queen_squares[rank][file] * adjFactorPos;
            }
            file++;
        }
    }
    return score;
}

float kingScore(Position& position) {
    float score = 0;
    string boardFen = position.fen();
    return score;
}

float mobilityScore(Position& position) {
    float score = 0;
    MoveList<WHITE> listWhiteMoves(position);
    score = listWhiteMoves.size() * 0.05;
    MoveList<BLACK> listBlackMoves(position);
    score = score - listBlackMoves.size() * (0.05);
    return score;
}

float coordScore(Position& position) {
    return 0;
}



float pawnStructureScore(Position& position) {
    return 0;
}