// Base researched criteria (Material, King Safety, Space, Mobility (could be similar to space), Piece Coordination, Pawn structure)
// Personal additions: Forced combination advantage (search will do this), Piece positioning, passed pawns
// Will use principle evaluation ideas + extensions/customizations but with unique methods and weights
// Source: https://chessify.me/blog/chess-engine-evaluation
// Check pieces around the king
// https://www.netlib.org/utk/lsi/pcwLSI/text/node343.html#:~:text=King%20safety%20is%20evaluated%20by,in%20front%20of%20the%20king.


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
float materialScore(vector<vector<int>> position);
float kingScore(vector<vector<int>> position);
float mobilityScore(vector<vector<int>> position, int ind_d2pos, int ind_d3pos);
float coordScore(vector<vector<int>> position);
float piecePosScore(vector<vector<int>> position);
float pawnStructureScore(vector<vector<int>> position);

int main() {
    vector<vector<vector<vector<vector<int>>>>> pos = interpretD3PosTxt();
    for (int ind_d1pos = 0; ind_d1pos < pos.size(); ind_d1pos++) { // All positions in depth 0 (Moves for depth 1)
        for (int ind_d2pos = 0; ind_d2pos < pos[ind_d1pos].size(); ind_d2pos++) { // All positions in depth 1 (Moves for depth 2)
            for (int ind_d3pos = 0; ind_d3pos < pos[ind_d1pos][ind_d2pos].size(); ind_d3pos++) { // All positions in depth 2 (Moves for depth 3)
                for (const auto& row : pos[ind_d1pos][ind_d2pos][ind_d3pos]) {
                    for (int value : row) {
                        cout << value << " ";
                    }
                    cout << endl;
                }
                float evaluation = evaluatePos(pos[ind_d1pos][ind_d2pos][ind_d3pos], ind_d1pos, ind_d2pos);
                cout << "Eval: " << evaluation << " \n";
            }
        }
    }
    cout << pos.size();
    cout << pos[2].size();
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

    position_score = material_score + king_safety_score + mobility_score + coord_score + piece_pos_score;

    return position_score;

}

float materialScore(vector<vector<int>> position) {

    float score = 0;
    for (const auto& row : position) {
        for (int piece : row) {
            score = score + piece;
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
    float score = (numd3pos[ind_d1pos][ind_d2pos] - numd3pos[ind_d1pos].size()) * adjustment_factor;
    return score;
}

float coordScore(vector<vector<int>> position) {
    return 0;
}

float piecePosScore(vector<vector<int>> position) {
    float score = 0;
    vector<vector<float>> preferred_king_squares = {
        {0.15, 0.15, 0.1, 0.05, 0.05, 0.1, 0.15, 0.15},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {-0.15, -0.15, -0.1, -0.05, -0.05, -0.1, -0.15, -0.15}
    };

    vector<vector<float>> preferred_bishop_squares = {
        {0.15, 0.15, 0.1, 0.05, 0.05, 0.1, 0.15, 0.15},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {-0.15, -0.15, -0.1, -0.05, -0.05, -0.1, -0.15, -0.15}
    };

    for (int i = 0; i < position.size(); i++) {
        for (int j = 0; j < position[i].size(); j++) {
            if (position[i][j] == 1000 || position[i][j] == -1000) {
                score = score + preferred_king_squares[i][j];
            }
        }
    }
    return 0;
}

float pawnStructureScore(vector<vector<int>> position) {
    return 0;
}