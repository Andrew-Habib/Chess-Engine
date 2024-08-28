#include <iostream>
using namespace std;

// Base researched criteria (Material, King Safety, Space, Mobility (could be similar to space), Piece Coordination, Pawn structure)
// Personal additions: Forced combination advantage (search will do this), Piece positioning
// Will use principle evaluation ideas + extensions/customizations but with unique methods and weights
// Source: https://chessify.me/blog/chess-engine-evaluation

#include <iostream>
#include <fstream>
#include <vector>
#include <sstream>
using namespace std;

void getDepth3Positions();

int main() {
    getDepth3Positions();
    return 0;
}

void getDepth3Positions() { 
    ifstream file("../../../data.txt");
    string line;
    vector<vector<vector<int>>> d3Positions; // 3D vector to store arrays
    vector<vector<int>> currentArray;
    vector<int> currentRow;

    if (!file) {
        cerr << "Unable to open file";
    }

    while (getline(file, line)) {
        if (line.empty()) {
            continue; // Skip empty lines
        }

        if (line == "|") {
            // End of a 2D array
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

    // Add the last array if needed
    if (!currentArray.empty()) {
        d3Positions.push_back(currentArray);
    }

    file.close();

    // Print the d3Positions
    for (const auto& matrix : d3Positions) {
        for (const auto& row : matrix) {
            for (int value : row) {
                cout << value << " ";
            }
            cout << endl;
        }
        cout << endl;
    }

}

int evaluate_pos() {
    int move_score = 0;
    return 0;
}

int material_score() {
    return 0;
}

int king_score() {
    return 0;
}

int space_score() {
    return 0;
}

int coord_score() {
    return 0;
}

int piece_pos_score() {
    return 0;
}