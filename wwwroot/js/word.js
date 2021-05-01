"use strict";
var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();

// this function shows a message when a user connects the hub
connection.on("ShowUsers", function (data) {
   // a for loop that compares the previous element so it doesn't repeat and appends only the last element in the list
    for (var i = data.length - 1; i < data.length; i++) {
        if (data[i] != data[i - 1])
        {
            var li = document.createElement("li");
            var text = document.createTextNode(data[i].name + " has joined the lobby");
            li.appendChild(text);
            document.getElementById("userList").appendChild(li);
        }
    }
});

// receives data from a method in the hub.
connection.on("noOpponent", function (message) {
    var waitingModal = document.getElementById("searchOpponent");
    var text = document.createTextNode(message);
    document.getElementById("header").appendChild(text);
    waitingModal.classList.add('active');
});

// receives data from a method in the hub.
connection.on("info", function (message) {
    document.getElementById('info').innerHTML = message;
});

// receives data from a method in the hub.
connection.on("foundOpponent", function (message) {
    var waitingModal = document.getElementById("searchOpponent");
    var text = document.createTextNode(message);
    document.getElementById("header").appendChild(text);
    waitingModal.classList.add('active');
    x.style.display = "block";
    
});

// add a eventlistenter to a button, that invokes a method in the hub. 
document.getElementById("startGame").addEventListener("click", function (event) {
    connection.invoke("FindOpponent").catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

// register the player and add it to a list of players on connection by calling RegisterClient function
connection.start().then(function () {
    var user = document.getElementById("userInput").value;
    connection.invoke("RegisterClient", user);
    reset();
}).catch(function (err) {
    return console.error(err.toString());
});

//number of wrong guesses
let maxWrong = 6;

// Gets data from the session storage and uses it to populate our "mistakes" variable, in the if else statement.
let retrievedMistakes = sessionStorage.getItem("mistakesCount");
let mistakesSession = JSON.parse(retrievedMistakes);
if (mistakesSession) {
    mistakes = mistakesSession;
    document.getElementById('mistakes').innerHTML = mistakes;
}
else {
    var mistakes = 0;
};

// Gets data from the session storage and uses it to populate our "gusssed" array, in the if else statement.  
let retrievedguessed = sessionStorage.getItem("guesssedWords");
let guessedWordSession = JSON.parse(retrievedguessed);
if (guessedWordSession) {
    var guessed = guessedWordSession;
}
else {
    var guessed = [];
};

// Gets data from the session storage and uses it to populate our answer variable, in the if else statement
let retrievedWord = sessionStorage.getItem("wordSession");
let wordSession = JSON.parse(retrievedWord);
if (wordSession) {
    var answer = wordSession;
}
else {
    setTimeout(randomWord, 500); //We use a timeout because the hub connectionion isnt always finished at this point. 
};

// Gets data from the session storage and uses it to populate our wordStatus variable, in the if else statement
let retrievedwordStatus = sessionStorage.getItem("wordStatusSession");
let wordStatusSession = JSON.parse(retrievedwordStatus);
if ((wordStatusSession === null) && (wordStatusSession === answer)) {
    wordStatus = null;
}
else {
    var wordStatus = wordStatusSession;
};

/* This function picks a random word from our array which is pulled from the database. 
 * Here we are also invoking a method in the hub that sends the generated word to the other clients */
function randomWord() {
    answer = myArray[Math.floor(Math.random() * myArray.length)];
    sessionStorage.setItem('wordSession', JSON.stringify(answer));

    connection.invoke("SendAnswer", answer).catch(function (err) {
        return console.error(err.toString());
    });
}

/* Here we are taking the generated answer from previous function, and splitting it into "_".
 * If the "guessed" array contains a letter which is equal to one of the letters in the "answer" variable, it
 * shows the letter instead of a "_".
 */
function guessedWord() {

    connection.on("ReceiveAnswer", function (shareAnswer) {
        answer = shareAnswer;
        sessionStorage.setItem('wordSession', JSON.stringify(answer));
    });

    wordStatus = answer.split('').map(letter => (guessed.indexOf(letter) >= 0 ? letter : " _ ")).join('');

    console.log(guessed);

    // storing the word in the session Storage. 
    document.getElementById('wordSpotlight').innerHTML = wordStatus;
    sessionStorage.setItem('wordStatusSession', JSON.stringify(wordStatus));

    //invoking a method from the hub, that shares the "wordStatus" with the other clients. 
    connection.invoke("SendWordStatus", wordStatus).catch(function (err) {
        return console.error(err.toString());
    });

}

// we take the alphabet string, divide it by letter and for every letter we generate a button in DOM
function generateButtons() {
    let buttonsHTML = 'abcdefghijklmnopqrstuvwxyz'.split('').map(letter =>
        `
      <button class="btn btn-lg btn-primary m-2" id='` + letter + `' onClick="handleGuess('` + letter + `')">` + letter + `</button>`).join('');

    connection.invoke("SendKeyboard", buttonsHTML).catch(function (err) {
        return console.error(err.toString());
    });

}

/* execute when a keyboard button is pressed, here we take the pressed letter as a parameter, check whether our "guessed" array 
 contains the guessed letter, if no we push it in and disable that button*/
function handleGuess(chosenLetter) {
    guessed.indexOf(chosenLetter) === -1 ? guessed.push(chosenLetter) : null;

    console.log(chosenLetter);

    document.getElementById(chosenLetter).setAttribute('disabled', true);
    sessionStorage.setItem('guesssedWords', JSON.stringify(guessed));

    connection.invoke("SendGuessedLetter", guessed, chosenLetter).catch(function (err) {
        return console.error(err.toString());
    });

    connection.invoke("Play").catch(function (err) {
        return console.error(err.toString());
    });
/* we check whether our answer variable which contains our word to be guessed */
    if (answer.indexOf(chosenLetter) >= 0) {
        guessedWord();
        checkIfGameWon();
    } else if (answer.indexOf(chosenLetter) === -1) {
        mistakes++;
        updateMistakes();
        checkIfGameLost();
        updatePic();
        //updateHangmanPicture();
    }
}

/* we update our mistakes count  */
function updateMistakes() {
    mistakes;
    sessionStorage.setItem('mistakesCount', JSON.stringify(mistakes))

    connection.invoke("SendMistakes", mistakes).catch(function (err) {
        return console.error(err.toString());
    });

}

// Checks if the wordStatus is equal to the answer, if so, the game should display that the users has won. 
function checkIfGameWon() {
    if (wordStatus === answer) {
        document.getElementById('keyboard').innerHTML = '';
        document.getElementById('info').innerHTML = "You Won!!!";

        sessionStorage.clear();
    }
}

// checks if the amount of mistakes is equal to the maxWrong variable, if so, it should display that the users has lost. 
function checkIfGameLost() {
    if (mistakes === maxWrong) {
        document.getElementById('wordSpotlight').innerHTML = 'The answer was: ' + answer;

        document.getElementById('keyboard').innerHTML = '';
        document.getElementById('info').innerHTML = "You Lost!!!";

        sessionStorage.clear();
    }
}

// Updates the hangman picture, which is equal to amount of mistakes that has been made. 
function updatePic() {
    document.getElementById('hangmanPic').src = '/images/' + mistakes + '.jpg';
}

//receive data from a method in the hub, that updates the number of mistakes on all clients.
connection.on("ReceiveMistakes", function (shareMistake) {
    mistakes = shareMistake;
    document.getElementById('mistakes').innerHTML = mistakes;
    checkIfGameLost();
    updatePic();
});

//receive data from a method in the hub, that shares the wordStatus with all the clients. 
connection.on("ReceiveWordStatus", function (shareWordStatus) {
    wordStatus = shareWordStatus;
    sessionStorage.setItem('wordStatusSession', JSON.stringify(wordStatus));
    document.getElementById('wordSpotlight').innerHTML = shareWordStatus;

    checkIfGameWon();
});

//receive data from a method in the hub, that shares the guessed array with all the clients.
connection.on("ReceiveGuessedLetter", function (shareGuessed, shareLetter) {
    guessed = shareGuessed;
    sessionStorage.setItem('guesssedWords', JSON.stringify(guessed));
    document.getElementById(shareLetter).setAttribute('disabled', true);
});

//receive data from a method in the hub, so that the users always has the same keyboard. 
connection.on("ReceiveKeyboard", function (shareKeyboard) {
    document.getElementById('keyboard').innerHTML = shareKeyboard;
    guessed.forEach(letter => document.getElementById(letter).setAttribute('disabled', true));
});

//receive data from a method in the hub, knowing if the users should have their keyboard disabled. 
connection.on("disableKey", function () {
    document.getElementById("keyboard").style.display = "none";
});

//receive data from a method in the hub, knowing if the users should have their keyboard enabled.  
connection.on("enableKey", function () {
    document.getElementById("keyboard").style.display = "block";
});

// This function calls many of the previous functions in order to reset the game. 
function reset() {

    guessed = [];
    connection.invoke("ResetGuessed", guessed).catch(function (err) {
        return console.error(err.toString());
    });

    connection.on("ReceiveResetGuessed", function (resetGuessed) {
        document.getElementById('info').innerHTML = "";
        guessed = resetGuessed;
        guessed.forEach(letter => document.getElementById(letter).setAttribute('disabled', true));
    });

    mistakes = 0;
    
    sessionStorage.clear();

    randomWord();
    guessedWord();
    updateMistakes();
    generateButtons();
    updatePic()
}

updatePic()
document.getElementById('maxWrong').innerHTML = maxWrong;