# CS455 Module 3 PR1: ML Agents++
Author: Georgiy Antonovich Bondar  

Go to https://keshakot.github.io/CS455_M3.PR1/ to play the game)

# Behaviors
ML Agent : Assets/Scripts/ShootTargetAgent.cs

# Description
1. The agent needs to fire projectiles to hit the white moving targets which spawn along the edges of the game board.  
2. The agent uses a neural network to decide where to aim to hit the targets. The neural network takes as its inputs a 50x50 monochrome camera feed and outputs an x and y firing coordinate, where the projectile is then fired.  
3. To train the neural network, the following weights were given: to fire a projectile is -0.01 reward, to hit the target is +10 reward, and to let the target reach the edge of the board is -10 reward.  
4. The network was trained by reinforcement learning with the following curriculum:   
	a. First, train it to hit stationary targets spawning in random locations  
	b. Second, train it to hit slow-moving targets spawning in random locations  
	c. Finally, train it to hit the faster-moving projectiles spawning on the edges of the board.  

# Notes
1. The current NN is not perfect: it will hit most targets, but will miss some. The tensorflow graph of the network's learning is below.  
![tensorboard data](/tensorboard.png?raw=true "tensorflow data for the NN")  
2. Attempts to add a fire/not fire output to the NN were made - this would have resulted in a 'cleaner' look to the AI since it would not be firing unless there was something to fire at. This was abandoned, for in training the NN was prone, when for a period unsuccessful at hitting the target, to stop firing at all and simply let the targets reach their goals. Since the AI was missing so much, the losses incurred by attempting to shoot were deemed to great to even make the attempt; in short, the AI would get trapped in this well of a local minimum loss and fail to escape. Given more training time and ingenuity this could have been surely amended, however.  


