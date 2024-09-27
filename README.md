<h1>Neural Network</h1>
<p>by <b>Antoine Mordant</b></p>

This project is a network project made with <i>Unity 2022.3.10f1</i> in C# at ISART DIGITAL.

<h2> Goals </h2>

<p>
My goal was to create an artificial intelligence, a neural network, which should be able to play a simple 2D platformer game.
</p>

<h2> Work Process </h2>

<h3> Theory </h3>

At first, i had to choose which type of neural network i would build. Because of health problems and my part-time internship, i only had a week to go through this project, so i chose the model which was the most documented in our ai lesson, the ANN with back propagation.

For this kind of project, using a genetic algorythm or reinforcement programming would have been more efficient but i didn't have enough time for it.

<h3> Network Set-up </h3>

I based my network architecture on the ai lessons i received and an example project my professor gave me of a neural network learning to resolve XOR. Moreover I reprogrammed it from scratch to understand it more deeply.

This model is really simple : You set up the inputs, hidden layers and outputs as you need, then you give it a database. This base is composed of differents situations and their wanted outputs. 
The AI will get the situation as inputs and compute some outputs. If these outputs are different of those wanted, it will change the weight of the neurons to get a better result next time.

<h3> Database </h3>

The real problem was to get a data base that represents exactly the different situtations that could be goufn in the little platformer level that i created. It is impossible to find it online and the only real solution was to create my own.

To do so, i created a player with a data recorder on it. This data recorder is a hitbox that will record every gameobject that will enter it and sort it in different types : platforms, enemies and killzones. It will also record the player state (if it touches the ground...) and the distance to the end. All of it will create a situation, a list of input for the network.

Last of all, it will record the input the player enter to create the wanted output of the situation and save everything in a text file.

    The gameobjects are not saved completely, only the needed characteristics are saved. For example : only the position from the player in 2D and the global 2D scale (x, y) are saved for a platform. And of course, the number of each type of object on each frame are saved.

A full playthrough recorded gave me a database of around 2000 situations and corresponding outputs.

But a second problem appears : the number of input of the AI must always be the same and the recorder doesn't detect always the same amount of gameobject. So i had to determine the maximum of each type of gameobject it will record on a frame during a playthrough and made it the number of input of my network. On the premade level, the network has 41 inputs.

<h3> Hyper parameters </h3>

Another question appears : Which dimensions does my network need to be efficient ?
To be honest, i didn't implemented anything to calculate it, i just tried different setups.

<h3>Results</h3>

Unfortunately, i did not succed to create a good AI for my platformer. Through training, i obtained AI which run in the right way or jump continuesly in the right direction but i didn't succeed to get an AI that would clearly adapt to its environment and make moves from it. More than that, i lost them through a problem from the network loading system.

<h2> How to use the project ? </h2>

<h3> Training Scene </h3>

<img src = "Images_README/TrainingScene.png">

This is the main scene of the project. It is divided in 2 parts :

<b>The AI</b>

If you click on the Spawn AI button, you will instantiate an AI. It will unlock 5 buttons.

<img src = "Images_README/AIButton.png">

<b>The Create New button </b> will generate your neural network. If you want to personnalise it, you can change its settings once you spawn the AI.

<img src = "Images_README/NetworkSettings.png">

You can change the activation function, the number of hidden layer and the number of perceptron within them etc.

    Be careful, the number of input perceptron and ouput perceptron will be setup automatically. Also do not change these parameters after creating the network, it will not change in real time and probably break everything.

<b>The Train button </b>will train the AI with the chosen data base the number of time entered in the input field next to it (the default value is 1000).

<img src = "Images_README/DatabaseFile.png">

You can change the Database file here if you want.

    The training take a lot of time : for 1000 iterations, with a network with 2 hidden layers of 16 and 10 perceptrons, it will takes around 2 or 3 minutes and it goes exponentially.

<b> The Load button </b> will load a saved network from a text file.

<img src = "Images_README/NetworkSavedFile.png">

<b> The Play button </b> will enable the in-game behavior of the AI. It will detect its environment as input and compute it through the network to get its outputs.

Lastly, <b> the Save button </b> will save the current network in TrainedAI.txt in the Assets file of the Unity project. 

    If the file already exists, it will override it.

<b>The Player</b>

If you click on the Spawn Player button, you will instantiate a Player.

To control it :
 - Q/D : Movement right or left
 - SPACE : Jump

Once the player spawned, it unlocks 2 buttons :

<img src = "Images_README/PlayerButton.png">

<b>The Record button </b> starts the data recording of your gameplay and will add it to the database once you die or reach the end.

<b>The Revive button </b>enables the player once he is dead or he has reached the end.

<h2> What could be improved ? </h2>

<h3> Network model </h3>

The backpropagation model is not the most efficient one for a platformer.

A genetic algorithm or a fusion of the two would have been more efficient.

    Genetic algorithm is based on the natural evolution. You display many AI on your level and take the most efficient ones. Then you make them "mutate" and "breed" together to create new AIs from them and do that again and again to get a truly efficient one.

Even better, a reinforcement algorithm would have been perfect but it is harder to create.

    Reinforcement programming is the principal of a self learning AI. Thanks to a reward function, it allows your AI to learn by itself the different actions and behaviors you want it to learn.

<h3> Hyperparameters </h3>

The hyperparameters have a main role in the network efficiency. Determining it randomly is in fact a waste of performance so studying it more deeply would be necessary.

I could even integrate a algorithm to determine them automatically, as the bayesian optimisation.

    Bayesian optimisation is an algorithm that uses probability models to find the best hyperparameters.

<h3> Training time </h3>

The AI i trained the most was trained around 20000 times with a database with around 2000 situations. Perhaps, this was not nearly enough and it needed a lot more time to learn all this different situations. I was scared of over-train it which, form what i heard and learn, can be a big issue for neural network. But in fact, i may not have trained them nearly enough to get real results.

<h3> Data base </h3>

Maybe my data base wasn't enough. I could enlarge it by playing more than one playthrough or even playing on different levels to broaden the possible situations.

<h3>Debug</h3>

Developping a debug tool for the network would be helpful. For example, i have network loading problems that would have been easily solved with a visual debug for the network.