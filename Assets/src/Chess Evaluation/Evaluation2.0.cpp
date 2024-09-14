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
#include <unordered_set>
#include <unordered_map>

#include "./surge/src/position.h"
#include "./surge/src/tables.h"
#include "./surge/src/types.h"

using namespace std;


bool whiteTurn = true; // Is it white or black's turn
//vector<vector<vector<int>>> d3Positions; // All positions at Depth of 3 nodes in a tree
vector<vector<int>> numd3pos; // Number of moves possible at Depth 2. .size for number of depth 1 positions
unordered_map<uint64_t, float> transpositionTable;

//d2 has how many d3 and d1 has how many d2

void generate_positions(Position& p, int depth, unordered_set<string>& positions);
float evaluatePos(vector<vector<int>> position, int ind_d2pos, int ind_d3pos);
float alphaBetaPruneD3Positions(vector<vector<vector<vector<vector<int>>>>> pos);
float materialScore(vector<vector<int>> position);
float kingScore(vector<vector<int>> position);
float mobilityScore(vector<vector<int>> position, int ind_d2pos, int ind_d3pos);
float coordScore(vector<vector<int>> position);
float generalPiecePosScore(vector<vector<int>> position);
float pawnStructureScore(vector<vector<int>> position);

int main() {
    initialise_all_databases();
    zobrist::initialise_zobrist_keys();

    Position p;
    p.set("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq -", p);  // Set the starting position

    int depth = 4;
    unordered_set<string> all_positions;

    generate_positions(p, depth, all_positions);

    cout << "Total positions at depth " << depth << ": " << all_positions.size() << std::endl;

    //// Output the positions (in FEN format)
    //for (const string& fen : all_positions) {
    //    cout << fen << endl;
    //}

    cout << all_positions.size() << endl;

    return 0;
}

void generate_positions(Position& p, int depth, unordered_set<string>& positions) {

    if (depth == 0) {
        positions.insert(p.fen());
        return;
    }

    uint64_t zKey = p.get_hash();

    if (transpositionTable.find(zKey) != transpositionTable.end()) {
       /* std::cout << "Position already evaluated with score: " << transpositionTable[zKey] << std::endl;*/
    } else {
        float evaluation = 0;
        transpositionTable[zKey] = evaluation;
        /*std::cout << "Evaluated position with score: " << evaluation << std::endl;*/
    }

    if (p.turn() == Color::WHITE) {
        MoveList<WHITE> list(p);
        for (Move m : list) {
            p.play<WHITE>(m);
            generate_positions(p, depth - 1, positions);
            p.undo<WHITE>(m);
        }
    } else {
        MoveList<BLACK> list(p);
        for (Move m : list) {
            p.play<BLACK>(m);
            generate_positions(p, depth - 1, positions);
            p.undo<BLACK>(m);
        }
    }

}

float evaluatePos(vector<vector<int>> position, int ind_d2pos, int ind_d3pos) {

    float position_score = 0;

    float material_score = materialScore(position);
    float king_safety_score = kingScore(position);
    float mobility_score = mobilityScore(position, ind_d2pos, ind_d3pos);
    float coord_score = coordScore(position);
    float general_piece_pos_score = generalPiecePosScore(position);
    float pawn_structure_score = pawnStructureScore(position);

    position_score = material_score + king_safety_score + mobility_score + coord_score + general_piece_pos_score + pawn_structure_score;

    return position_score;

}

float alphaBetaPruneD3Positions(vector<vector<vector<vector<vector<int>>>>> pos) {
    
    float alpha1 = -100000;
    float beta1 = 100000;
    float extEval1 = whiteTurn ? -100000 : 100000;
    vector<vector<int>> bestPos1;
    vector<vector<int>> bestPos2;
    vector<vector<int>> bestPos3;

    for (int ind_d1pos = 0; ind_d1pos < pos.size(); ind_d1pos++) {

        float alpha2 = -100000;
        float beta2 = 100000;
        float extEval2 = !whiteTurn ? -100000 : 100000;

        for (int ind_d2pos = 0; ind_d2pos < pos[ind_d1pos].size(); ind_d2pos++) {

            float alpha3 = -100000;
            float beta3 = 100000;
            float extEval3 = whiteTurn ? -100000 : 100000;

            for (int ind_d3pos = 0; ind_d3pos < pos[ind_d1pos][ind_d2pos].size(); ind_d3pos++) {
                float eval3 = evaluatePos(pos[ind_d1pos][ind_d2pos][ind_d3pos], ind_d1pos, ind_d2pos);
                extEval3 = whiteTurn ? max(alpha3, eval3) : min(beta3, eval3);

                if (whiteTurn) {
                    if (eval3 > alpha3) {
                        bestPos3 = pos[ind_d1pos][ind_d2pos][ind_d3pos];
                        alpha3 = eval3;
                    }
                } else {
                    if (eval3 < beta3) {
                        bestPos3 = pos[ind_d1pos][ind_d2pos][ind_d3pos];
                        beta3 = eval3;
                    }
                }

                if (beta3 <= alpha3) {
                    break;
                }
            }

            float eval2 = extEval3;
            if ((!whiteTurn && eval2 > alpha2) || (whiteTurn && eval2 < beta2)) {
                bestPos2 = bestPos3;
                extEval2 = eval2;

                if (!whiteTurn) {
                    alpha2 = eval2;
                } else {
                    beta2 = eval2;
                }
            }

            if (beta2 <= alpha2) {
                break;
            }
        }

        float eval1 = extEval2;
        if ((whiteTurn && eval1 > alpha1) || (!whiteTurn && eval1 < beta1)) {
            bestPos1 = bestPos2;
            extEval1 = eval1;

            if (whiteTurn) {
                alpha1 = eval1;
            } else {
                beta1 = eval1;
            }
        }

        if (beta1 <= alpha1) {
            break;
        }
    }

    for (const auto& row : bestPos1) {
        for (int value : row) {
            cout << value << " ";
        }
        cout << endl;
    }

    return extEval1;

}

float materialScore(vector<vector<int>> position) {

    float score = 0;
    for (const auto& row : position) {
        for (int piece : row) {
            if (piece != 4 || piece != -4) {
                score = score + piece;
            } else if (piece == 4) {
                score = score + 3.15;
            } else if (piece == -4) {
                score = score - 3.15;
            }
        }
    }
    return score;

}

float kingScore(vector<vector<int>> position) {
    float score = 0;
    
    return score;
}

float mobilityScore(vector<vector<int>> position, int ind_d1pos, int ind_d2pos) {
    float adjustment_factor = whiteTurn ? 0.1 : -0.1;
    float score = 0;
    return score;
}

float coordScore(vector<vector<int>> position) {
    return 0;
}

float generalPiecePosScore(vector<vector<int>> position) {
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

    for (int i = 0; i < position.size(); i++) {
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
    }
    return score;
}

float pawnStructureScore(vector<vector<int>> position) {
    return 0;
}