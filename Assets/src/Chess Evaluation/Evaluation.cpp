// Base researched criteria (Material, King Safety, Space, Mobility (could be similar to space), Piece Coordination, Pawn structure)
// Personal additions: Forced combination advantage (search will do this), Piece positioning, passed pawns
// Will use principle evaluation ideas + extensions/customizations but with unique methods and weights
// Source: https://chessify.me/blog/chess-engine-evaluation

#include <iostream>
#include <fstream>
#include <vector>
#include <sstream>
using namespace std;

vector<vector<vector<int>>> getDepth3Positions();
float evaluatePos(vector<vector<int>> position);
float materialScore(vector<vector<int>> position);
float kingScore(vector<vector<int>> position);
float mobilityScore(vector<vector<int>> position);
float coordScore(vector<vector<int>> position);
float piecePosScore(vector<vector<int>> position);
float pawnStructureScore(vector<vector<int>> position);

bool whiteTurn = true;
vector<vector<vector<int>>> d3Positions;

int main() {
    d3Positions = getDepth3Positions();
    for (const auto& position : d3Positions) {
        /*for (const auto& row : position) {
            for (int value : row) {
                cout << value << " ";
            }
            cout << endl;
        }*/
        float evaluation = evaluatePos(position);
        /*cout << "Eval: " << evaluation << " \n";*/
    }
    
    return 0;
}

vector<vector<vector<int>>> getDepth3Positions() { 
    ifstream file("../../../data.txt");
    string line;
    vector<vector<vector<int>>> d3Positions;
    vector<vector<int>> currentArray;
    vector<int> currentRow;

    vector<int> numMovesD2;
    int movesCurrD2Pos;

    if (!file) {
        cerr << "Unable to open file";
    }

    while (getline(file, line)) {
        if (line.empty()) {
            continue;
        }

        if (line.find("10") != string::npos) {
            whiteTurn = true;
        } else if (line.find("-10") != string::npos) {
            whiteTurn = false;
        }
        
        if (line.find("2") != string::npos) {
            numMovesD2.push_back(movesCurrD2Pos);
            cout << movesCurrD2Pos << "\n";
            movesCurrD2Pos = 0;
            continue;
        }

        if (line == "|") {
            movesCurrD2Pos++;
            if (!currentArray.empty()) {
                d3Positions.push_back(currentArray);
                currentArray.clear();
            }
            continue;
        }

        istringstream iss(line);
        string value;
        currentRow.clear();
        while (getline(iss, value, ',')) {
            currentRow.push_back(stoi(value));
        }

        if (!currentArray.empty() && currentRow.size() == currentArray[0].size()) {
            currentArray.push_back(currentRow);
        }
        else {
            if (!currentArray.empty()) {
                d3Positions.push_back(currentArray);
            }
            currentArray.clear();
            currentArray.push_back(currentRow);
        }
    }

    if (!currentArray.empty()) {
        d3Positions.push_back(currentArray);
    }

    file.close();

    return d3Positions;

}

float evaluatePos(vector<vector<int>> position) {

    float position_score = 0;

    float material_score = materialScore(position);
    float king_safety_score = kingScore(position);
    float mobility_score = mobilityScore(position);
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
    // Check pieces around the king
    // https://www.netlib.org/utk/lsi/pcwLSI/text/node343.html#:~:text=King%20safety%20is%20evaluated%20by,in%20front%20of%20the%20king.

    return score;
}

float mobilityScore(vector<vector<int>> position) {
    return 0;
}

float coordScore(vector<vector<int>> position) {
    return 0;
}

float piecePosScore(vector<vector<int>> position) {
    int score = 0;
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