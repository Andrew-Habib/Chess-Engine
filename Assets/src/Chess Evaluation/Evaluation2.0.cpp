// Base researched criteria (Material, King Safety, Space, Mobility (could be similar to space), Piece Coordination, Pawn structure)
// Personal additions: Forced combination advantage (search will do this), Piece positioning, passed pawns
// Will use principle evaluation ideas + extensions/customizations but with unique methods and weights
// https://chatgpt.com/ used for idea generation for optimizations
// Credits to https://github.com/nkarve/surge for fast bitboard legal chess move generator and zobrist hashing (Stockfish Wrapper)
// Used Stockfish Engine 17 Open Source Chess Engine for efficient UCI and benchmarking https://stockfishchess.org/
// Source: https://chessify.me/blog/chess-engine-evaluation
// Check pieces around the king
// Alpha beta pruning + minimax aglorithm https://www.youtube.com/watch?v=l-hh51ncgDI

#include <iostream>
#include <fstream>
#include <vector>
#include <sstream>
#include <cmath>
#include <string>
#include <algorithm>
#include <unordered_set>
#include <unordered_map>

#include "./surge/src/position.h"
#include "./surge/src/tables.h"
#include "./surge/src/types.h"

using namespace std;

bool whiteTurn = true; // Is it white or black's turn
unordered_map<uint64_t, float> transpositionTable;

float alphabetaPrunePositions(Position& p, int depth, unordered_set<string>& positions, float alpha, float beta);
float evaluatePos(Position& position);
float materialScore(Position& position);
float kingScore(Position& position);
float mobilityScore(Position& position);
float coordScore(Position& position);
float generalPiecePosScore(Position& position);
float pawnStructureScore(Position& position);

int main() {
    initialise_all_databases();
    zobrist::initialise_zobrist_keys();

    Position p;
    p.set("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq -", p);

    whiteTurn = p.turn() == Color::WHITE;

    int depth = 10;
    unordered_set<string> all_positions;

    alphabetaPrunePositions(p, depth, all_positions, -100000, 100000);

    cout << "Total positions at depth " << depth << ": " << all_positions.size() << std::endl;

    // Output the positions (in FEN format)
    /*for (const string& fen : all_positions) {
        cout << fen << endl;
    }*/

    cout << all_positions.size() << endl;

    return 0;
}

float alphabetaPrunePositions(Position& p, int depth, unordered_set<string>& positions, float alpha, float beta) {

    if (depth == 0) {
        positions.insert(p.fen());
        return evaluatePos(p);
    }

    uint64_t zKey = p.get_hash();

    if (transpositionTable.find(zKey) != transpositionTable.end()) {
        return transpositionTable[zKey];
    }

    if (p.turn() == Color::WHITE) { // Max Player
        float extEval = -100000;
        MoveList<WHITE> list(p);
        for (Move m : list) {
            p.play<WHITE>(m);
            float eval = alphabetaPrunePositions(p, depth - 1, positions, alpha, beta);
            extEval = max(extEval, eval);
            alpha = max(alpha, extEval);
            p.undo<WHITE>(m);
            if (beta <= alpha) break;    
        }
        return extEval;
    } else { // Min Player
        float extEval = 100000;
        MoveList<BLACK> list(p);
        for (Move m : list) {
            p.play<BLACK>(m);
            float eval = alphabetaPrunePositions(p, depth - 1, positions, alpha, beta);
            extEval = min(extEval, eval);
            beta = min(beta, extEval);
            p.undo<BLACK>(m);
            if (beta <= alpha) break;            
        }

        transpositionTable[zKey] = extEval;
        return extEval;
    }

}

float evaluatePos(Position& position) {

    float position_score = 0;

    /*float material_score = materialScore(position);
    float king_safety_score = kingScore(position);
    float mobility_score = mobilityScore(position);
    float coord_score = coordScore(position);
    float general_piece_pos_score = generalPiecePosScore(position);
    float pawn_structure_score = pawnStructureScore(position);

    position_score = material_score + king_safety_score + mobility_score + coord_score + general_piece_pos_score + pawn_structure_score;*/

    return position_score;

}

float materialScore(Position& position) {

    float score = 0;
    string positionFen = position.fen();
    return score;

}

float kingScore(Position& position) {
    float score = 0;
    
    return score;
}

float mobilityScore(Position& position) {
    float adjustment_factor = whiteTurn ? 0.1 : -0.1;
    float score = 0;
    return score;
}

float coordScore(Position& position) {
    return 0;
}

float generalPiecePosScore(Position& position) {
    float score = 0;
    vector<vector<float>> preferred_white_king_squares = {
        {0.2, 0.25, 0.1, 0, 0, 0.1, 0.25, 0.2},
        {-0.2, -0.2, -0.2, -0.2, -0.2, -0.2, -0.2, -0.2},
        {-0.5, -0.5, -0.5, -0.25, -0.25, -0.5, -0.5, -0.5},
        {-1, -1, -1, -1, -1, -1, -1, -1},
        {-2, -2, -2, -2, -2, -2, -2, -2},
        {-3, -3, -3, -3, -3, -3, -3, -3},
        {-4, -4, -4, -4, -4, -4, -4, -4},
        {-5, -5, -5, -5, -5, -5, -5, -5}
    };

    vector<vector<float>> preferred_black_king_squares = {
        {5, 5, 5, 5, 5, 5, 5, 5},
        {4, 4, 4, 4, 4, 4, 4, 4},
        {3, 3, 3, 3, 3, 3, 3, 3},
        {2, 2, 2, 2, 2, 2, 2, 2},
        {1, 1, 1, 1, 1, 1, 1, 1},
        {0.5, 0.5, 0.5, 0.25, 0.25, 0.5, 0.5, 0.5},
        {0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2},
        {-0.2, -0.25, -0.1, 0, 0, -0.1, -0.25, -0.2}
    };

    vector<vector<float>> preferred_white_bishop_squares = {
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0.1, 0.25, 0.1, 0.15, 0.15, 0.1, 0.25, 0.1},
        {0.1, 0.1, 0.15, 0.2, 0.2, 0.15, 0.1, 0.1},
        {0.15, 0.15, 0.2, 0.2, 0.2, 0.2, 0.15, 0.15},
        {0.15, 0.15, 0.2, 0.2, 0.2, 0.2, 0.15, 0.15},
        {0.1, 0.1, 0.1, 0.15, 0.15, 0.1, 0.1, 0.1},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0}
    };

    vector<vector<float>> preferred_black_bishop_squares = {
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {-0.1, -0.1, -0.1, -0.15, -0.15, -0.1, -0.1, -0.1},
        {-0.15, -0.15, -0.2, -0.2, -0.2, -0.2, -0.15, -0.15},
        {-0.15, -0.15, -0.2, -0.2, -0.2, -0.2, -0.15, -0.15},
        {-0.1, -0.1, -0.15, -0.2, -0.2, -0.15, -0.1, -0.1},
        {-0.1, -0.25, -0.1, -0.15, -0.15, -0.1, -0.25, -0.1},
        {0, 0, 0, 0, 0, 0, 0, 0}
    };

    vector<vector<float>> preferred_white_knight_squares = {
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0.05, 0.05, 0.1, 0.1, 0.05, 0.05, 0},
        {0.1, 0.15, 0.2, 0.15, 0.15, 0.2, 0.15, 0.1},
        {0.1, 0.15, 0.15, 0.2, 0.2, 0.15, 0.15, 0.1},
        {0.1, 0.2, 0.15, 0.2, 0.2, 0.15, 0.2, 0.1},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0}
    };

    vector<vector<float>> preferred_black_knight_squares = {
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {-0.1, -0.2, -0.15, -0.25, -0.25, -0.15, -0.2, -0.1},
        {-0.1, -0.15, -0.15, -0.25, -0.25, -0.15, -0.15, -0.1},
        {-0.1, -0.15, -0.25, -0.15, -0.15, -0.25, -0.15, -0.1},
        {0, -0.05, -0.05, -0.1, -0.1, -0.05, -0.05, 0},
        {0, 0, 0, 0, 0, 0, 0, 0}
    };

    vector<vector<float>> preferred_white_rook_squares = {
        {0.1, 0.1, 0.15, 0.2, 0.2, 0.15, 0.1, 0.1},
        {0.1, 0.1, 0.15, 0.2, 0.2, 0.15, 0.1, 0.1},
        {0.1, 0.1, 0.15, 0.2, 0.2, 0.15, 0.1, 0.1},
        {0.1, 0.1, 0.15, 0.2, 0.2, 0.15, 0.1, 0.1},
        {0.1, 0.1, 0.15, 0.2, 0.2, 0.15, 0.1, 0.1},
        {0.1, 0.1, 0.15, 0.2, 0.2, 0.15, 0.1, 0.1},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0}
    };

    vector<vector<float>> preferred_black_rook_squares = {
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {-0.1, -0.1, -0.15, -0.2, -0.2, -0.15, -0.1, -0.1},
        {-0.1, -0.1, -0.15, -0.2, -0.2, -0.15, -0.1, -0.1},
        {-0.1, -0.1, -0.15, -0.2, -0.2, -0.15, -0.1, -0.1},
        {-0.1, -0.1, -0.15, -0.2, -0.2, -0.15, -0.1, -0.1},
        {-0.1, -0.1, -0.15, -0.2, -0.2, -0.15, -0.1, -0.1},
        {-0.1, -0.1, -0.15, -0.2, -0.2, -0.15, -0.1, -0.1}
    };

    vector<vector<float>> preferred_white_queen_squares = {
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0.1, 0.1, 0.1, 0.1, 0, 0},
        {0, 0, 0.1, 0.2, 0.2, 0.1, 0, 0},
        {0, 0, 0.1, 0.2, 0.2, 0.1, 0, 0},
        {0, 0, 0.1, 0.1, 0.1, 0.1, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0}
    };

    vector<vector<float>> preferred_black_queen_squares = {
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, -0.1, -0.1, -0.1, -0.1, 0, 0},
        {0, 0, -0.1, -0.2, -0.2, -0.1, 0, 0},
        {0, 0, -0.1, -0.2, -0.2, -0.1, 0, 0},
        {0, 0, -0.1, -0.1, -0.1, -0.1, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0}
    };

    vector<vector<float>> preferred_white_pawn_squares = {
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0.1, 0.2, 0.15, 0.15, 0.15, 0.15, 0.2, 0.1},
        {0.1, 0.15, 0.2, 0.25, 0.25, 0.2, 0.15, 0.1},
        {0.1, 0.15, 0.2, 0.25, 0.25, 0.2, 0.15, 0.1},
        {1, 1, 1, 1, 1, 1, 1, 1},
        {3, 3, 3, 3, 3, 3, 3, 3},
        {0, 0, 0, 0, 0, 0, 0, 0}
    };

    vector<vector<float>> preferred_black_pawn_squares = {
        {0, 0, 0, 0, 0, 0, 0, 0},
        {-1, -1, -1, -1, -1, -1, -1, -1},
        {-3, -3, -3, -3, -3, -3, -3, -3},
        {-0.1, -0.15, -0.2, -0.25, -0.25, -0.2, -0.15, -0.1},
        {-0.1, -0.15, -0.2, -0.25, -0.25, -0.2, -0.15, -0.1},
        {-0.1, -0.2, -0.15, -0.15, -0.15, -0.15, -0.2, -0.1},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0}
    };

    /*for (int i = 0; i < position.size(); i++) {
        for (int j = 0; j < position[i].size(); j++) {
            if (position[i][j] == 1) {
                score = score + preferred_white_pawn_squares[i][j];
            } else if (position[i][j] == -1) {
                score = score + preferred_black_pawn_squares[i][j];
            } else if (position[i][j] == 1000) {
                score = score + preferred_white_king_squares[i][j];
            } else if (position[i][j] == -1000) {
                score = score + preferred_black_king_squares[i][j];
            } else if (position[i][j] == 4) {
                score = score + preferred_white_bishop_squares[i][j];
            } else if (position[i][j] == -4) {
                score = score + preferred_black_bishop_squares[i][j];
            } else if (position[i][j] == 3) {
                score = score + preferred_white_knight_squares[i][j];
            } else if (position[i][j] == -3) {
                score = score + preferred_black_knight_squares[i][j];
            } else if (position[i][j] == 5) {
                score = score + preferred_white_rook_squares[i][j];
            } else if (position[i][j] == -5) {
                score = score + preferred_black_rook_squares[i][j];
            } else if (position[i][j] == 9) {
                score = score + preferred_white_queen_squares[i][j];
            } else if (position[i][j] == -9) {
                score = score + preferred_black_queen_squares[i][j];
            } 
        }
    }*/
    return score;
}

float pawnStructureScore(Position& position) {
    return 0;
}