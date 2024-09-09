// Base researched criteria (Material, King Safety, Space, Mobility (could be similar to space), Piece Coordination, Pawn structure)
// Personal additions: Forced combination advantage (search will do this), Piece positioning, passed pawns
// Will use principle evaluation ideas + extensions/customizations but with unique methods and weights
// Source: https://chessify.me/blog/chess-engine-evaluation
// Check pieces around the king
// https://www.netlib.org/utk/lsi/pcwLSI/text/node343.html#:~:text=King%20safety%20is%20evaluated%20by,in%20front%20of%20the%20king.
// Alpha beta pruning + minimax aglorithm https://www.youtube.com/watch?v=l-hh51ncgDI
/*for (const auto& row : pos[ind_d1pos][ind_d2pos][ind_d3pos]) {
    for (int value : row) {
        cout << value << " ";
    }
    cout << endl;
}*/

#include <iostream>
#include <fstream>
#include <vector>
#include <sstream>
#include <cmath>
using namespace std;

bool whiteTurn = true; // Is it white or black's turn
//vector<vector<vector<int>>> d3Positions; // All positions at Depth of 3 nodes in a tree
vector<vector<int>> numd3pos; // Number of moves possible at Depth 2. .size for number of depth 1 positions

//d2 has how many d3 and d1 has how many d2

vector<vector<vector<vector<vector<int>>>>> interpretD3PosTxt(); // Outer d1, 2nd d2 pos, 3rd d3 pos
float evaluatePos(vector<vector<int>> position, int ind_d2pos, int ind_d3pos);
float alphaBetaPruneD3Positions(vector<vector<vector<vector<vector<int>>>>> pos);
float materialScore(vector<vector<int>> position);
float kingScore(vector<vector<int>> position);
float mobilityScore(vector<vector<int>> position, int ind_d2pos, int ind_d3pos);
float coordScore(vector<vector<int>> position);
float piecePosScore(vector<vector<int>> position);
float pawnStructureScore(vector<vector<int>> position);

int main() {
    vector<vector<vector<vector<vector<int>>>>> pos = interpretD3PosTxt();
    float evalBest = alphaBetaPruneD3Positions(pos);
    cout << evalBest << "\n";

    for (int i = 0; i < numd3pos.size(); i++) {
        cout << "Depth 2 Moves for Position " << i + 1 << ": ";
        for (int j = 0; j < numd3pos[i].size(); j++) {
            cout << numd3pos[i][j] << " ";
        }
        cout << endl;
    }
    return 0;
}

vector<vector<vector<vector<vector<int>>>>> interpretD3PosTxt() {
    ifstream file("../../../data.txt");
    string line;

    vector<vector<vector<vector<vector<int>>>>> d0Positions;
    vector<vector<vector<vector<int>>>> d1Positions; // Outer vector for depths
    vector<vector<vector<int>>> d2Positions; // Vector for Depth 2 positions
    vector<vector<int>> d3Position; // 2D vector representing the board
    vector<int> currentRow; // 1D vector representing a row of the board
    
    vector<int> d1numd2;
    int d2numd3 = 0;

    if (!file) {
        cerr << "Unable to open file";
    }

    while (getline(file, line)) {
        if (line.empty()) {
            continue;
        }

        if (line.find("10") != string::npos) {
            whiteTurn = true;
        }
        else if (line.find("-10") != string::npos) {
            whiteTurn = false;
        }

        if (line.find("111") != string::npos) {
            numd3pos.push_back(d1numd2);
            if (!d1Positions.empty()) {
                d0Positions.push_back(d1Positions);
            }
            d1Positions.clear();
            d1numd2.clear();
            continue;
        }

        if (line.find("222") != string::npos) {
            d1numd2.push_back(d2numd3);
            if (!d2Positions.empty()) {
                d1Positions.push_back(d2Positions);
            }
            d2Positions.clear();
            d2numd3 = 0;
            continue;
        }

        if (line == "|") {
            d2numd3++;
            if (!d3Position.empty()) {
                d2Positions.push_back(d3Position);
            }
            d3Position.clear();
            continue;
        }

        istringstream iss(line);
        string value;
        currentRow.clear();
        while (getline(iss, value, ',')) {
            currentRow.push_back(stoi(value));
        }

        if (!d3Position.empty() && currentRow.size() == d3Position[0].size()) {
            d3Position.push_back(currentRow);
        }
        else {
            if (!d3Position.empty()) {
                d2Positions.push_back(d3Position);
            }
            d3Position.clear();
            d3Position.push_back(currentRow);
        }
    }

    if (!d3Position.empty()) {
        d2Positions.push_back(d3Position);
    }
    if (!d2Positions.empty()) {
        d1Positions.push_back(d2Positions);
    }

    file.close();

    return d0Positions;
}

float evaluatePos(vector<vector<int>> position, int ind_d2pos, int ind_d3pos) {

    float position_score = 0;

    float material_score = materialScore(position);
    float king_safety_score = kingScore(position);
    float mobility_score = mobilityScore(position, ind_d2pos, ind_d3pos);
    float coord_score = coordScore(position);
    float piece_pos_score = piecePosScore(position);
    float pawn_structure_score = pawnStructureScore(position);

    position_score = material_score + king_safety_score + mobility_score + coord_score + piece_pos_score + pawn_structure_score;

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

float piecePosScore(vector<vector<int>> position) {
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
        {0.15, 0.15, 0.25, 0.2, 0.2, 0.25, 0.15, 0.15},
        {0.15, 0, 0, 0, 0, 0, 0, 0.15},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0}
    };

    vector<vector<float>> preferred_black_bishop_squares = {
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0}
    };

    vector<vector<float>> preferred_white_knight_squares = {
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0.05, 0.05, 0.1, 0.1, 0.05, 0.05, 0},
        {0.1, 0.15, 0.25, 0.15, 0.15, 0.25, 0.15, 0.1},
        {0.1, 0.15, 0.15, 0.35, 0.35, 0.15, 0.15, 0.1},
        {0.1, 0.25, 0.15, 0.35, 0.35, 0.15, 0.25, 0.1},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0}
    };

    vector<vector<float>> preferred_black_knight_squares = {
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {-0.1, -0.25, -0.15, -0.35, -0.35, -0.15, -0.25, -0.1},
        {-0.1, -0.15, -0.15, -0.35, -0.35, -0.15, -0.15, -0.1},
        {-0.1, -0.15, -0.25, -0.15, -0.15, -0.25, -0.15, -0.1},
        {0, -0.05, -0.05, -0.1, -0.1, -0.05, -0.05, 0},
        {0, 0, 0, 0, 0, 0, 0, 0}
    };



    for (int i = 0; i < position.size(); i++) {
        for (int j = 0; j < position[i].size(); j++) {
            if (position[i][j] == 1000) {
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
            }
        }
    }
    return score;
}

float pawnStructureScore(vector<vector<int>> position) {
    return 0;
}