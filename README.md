# Welcome to Rasa DotNet!

Hi! This is definitely still a work in progress, so bear with me.

Is it bare or bear? Oh, who cares... 

# \~\~More coming soon\~\~
# The Project

The idea is that the C# MVC application handles the utterances, data processing, and displays everything to the users. 
We use Rasa_NLU for our backend handling of intents and entity extraction. Google it or search in github. 

Of course, not everyone needs question, answer and repeat, so we have to add another layer of logic. Yay! 

Just kidding, it turned my beautiful, clean, sexy project into a bowl of steaming spaghetti. At least its warm so I can have some warmth to keep me alive through the frigid winters where I live (Florida)...

But anyway, I added that other crap, and now we can override intents, allowing you to basically force the user to fill out a glorified form.

Don't worry, I did something similar to this at work and they added bitmojis, so this is at least better than that. *cries*, this is what I want to college to do.


## Damn, you think I know what Intents, Entities, and utterances are? SOOOO Naive

Great point random internet stranger! 

An **Intent** is what the user wants. So if I want a pizza or not pizza chatbot, I would define two intents. "pizza" and "notPizza". 

You add examples of what the user might say to your rasa bot intents. Check out their documentation for more. 

An **Entity** is a specific topic someone is wanting. To repurpose the above example, say we are creating a chatbot that IDs what food a user is looking for, we would create our intents, and make the food item into an entity. See Rasa documentation for how you'd do this. Rasa also supports regEx.... So have fun with that. 


**UTTERANCES**!!! That is where I come in, I have constructed a utterance styling similar to Rasa. Check out the examples I have made inside the project. Its in the /chatbot-demo/App_Data/rasa/utterances folder. 

You can add an array of strings and my program will choose one at random, creating a more human like appearance from the bot. This way it doesn't say the *same* thing each time. 

Inside the Json for utterances, there is a property called nextState, we can use this to override the next intent. 

For example, we ask a user "What is your favorite food?" and the user says "Pizza"

We don't want to score an intent based on pizza! That might have disasterous results. It could come up with anything.... Plus they could enter something other than pizza, or a food we haven't thought of. 

We would set a nextState and use that to override the next intent. We could define utterances for a "saidFavFood" intent that doesn't exist inside our rasa Intents files, once we detect the correct nextState we reset the intent to saidFavFood and cause the bot to say what we want. 

**Currently broken:** We can even use the extracted entity inside our utterance. More on this once I fix the broken shit.

