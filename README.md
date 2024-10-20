# Chess-Engine
### Tech Stack:
1) C#, Unity --> Used to build Chess board Functionality and UI
2) C++ --> Used to build primary Chess Engine for evluation (Used Minimax Algorithm, Alpha-Beta Pruning, Zobrist Hashing 85 billion to 35,000 node optimization)
3) Python (TensorFlow, Numpy) --> Used for Neural Network to teach model opening move theory
4) MongoDB -> Integrated in Python ML scripts and used to store high-level chess games
### Compile Chess Engine (Access Chess Engine Not ML Eval Only): 
1) cd to 'Assets/src/Chess Evaluation'
2) g++ -o Evaluation2.0 Evaluation2.0.cpp ./surge/src/*.cpp
3) ./Evaluation2.0 to run
### References:
- Chess Rules and Board implementation Research: https://en.wikipedia.org/wiki/Rules_of_chess
- Shannon's Number Research: https://en.wikipedia.org/wiki/Shannon_number
- Basics of Chess Evaluation: https://chessify.me/blog/chess-engine-evaluation
- Detailed Chess Evaluation Research: https://www.chessprogramming.org/Evaluation
- Stockfish Engine 17 Open Source Chess Engine for efficient UCI and benchmarking: https://stockfishchess.org/
- Fast bitboard legal chess move generator and zobrist hashing (Stockfish Wrapper): https://github.com/nkarve/surge
- Idea generation and optimization and Machine Learning ideas and Explanations: https://chatgpt.com
- Evaluation criteria: https://chessify.me/blog/chess-engine-evaluation
- Alpha beta pruning + minimax aglorithm: https://www.youtube.com/watch?v=l-hh51ncgDI
- Understanding of Alpha-Beta Pruning algorithm: https://www.geeksforgeeks.org/minimax-algorithm-in-game-theory-set-4-alpha-beta-pruning/
- Machine Learning Data High-level Chess Games: https://database.nikonoel.fr/
