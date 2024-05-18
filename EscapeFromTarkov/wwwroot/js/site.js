document.getElementById("more-btn").style.display = "none";
document.querySelector(".chat-input input").setAttribute("disabled", "disabled");
document.getElementById("send").setAttribute("disabled", "disabled");
document.getElementById("start-btn").addEventListener("click", function (event) {
    event.preventDefault();
    this.style.display = "none";
    document.querySelector(".chat-input input").removeAttribute("disabled");
    document.getElementById("send").removeAttribute("disabled");

    var chatMessages = document.querySelector(".chat-messages");
    var newMyMessage = createMessageElement("message", questions[currentQuestionIndex]);
    chatMessages.appendChild(newMyMessage);
    currentQuestionIndex++;
});


function createMessageElement(className, content, isTheir = false) {
    var messageElement = document.createElement("div");
    messageElement.className = "message " + className;
    messageElement.innerHTML = `<div class='avatar ${isTheir ? "their-avatar" : ""}'></div><div class='message-content ${isTheir ? "their" : ""}'>${content}</div>`;
    return messageElement;
}

function scrollToBottom(element) {
    element.scrollTop = element.scrollHeight;
}
var botDictionary = {
    "Какой товар вам необходимо найти?": ""
};
var currentQuestionIndex = 0;
var questions = Object.keys(botDictionary);
document.getElementById("send").addEventListener("click", function (event) {
    event.preventDefault();
    sendMessage();
});
document.querySelector(".chat-input input").addEventListener("keypress", function (e) {
    if (e.key === "Enter") {
        e.preventDefault();
        sendMessage();
    }
});
var totalDisplayedGifts = 0;

function getGiftsFromServer() {
    var chatMessages = document.querySelector(".chat-messages");
    $.ajax({
        type: "POST",
        url: "/GetProduct",
        contentType: "application/json",
        data: JSON.stringify(botDictionary),
        success: function (data) {
            if (data.totalCount === 0) {
                var userMessage = "Такого товара нет. Нажмите Еще, чтобы продолжить поиск";
                var newTheirMessage = createMessageElement("message", userMessage, false);
                chatMessages.appendChild(newTheirMessage);
            }
            else {
                var totalCount = data.totalCount;
                if (totalDisplayedGifts < totalCount) {
                    var textBlock = document.createElement("div");
                    textBlock.className = "message";

                    var avatar = document.createElement("div");
                    avatar.classList.add("avatar");
                    textBlock.appendChild(avatar);

                    var textBlockContentElement = document.createElement("div");
                    textBlockContentElement.classList.add("message-content", "gift");
                    textBlockContentElement.textContent = "Вот что мне удалось подобрать";
                    textBlock.appendChild(textBlockContentElement);
                    chatMessages.appendChild(textBlock);
                    data.gifts.forEach(function (gift) {
                        var giftMessage = createGiftMessageElement(gift);
                        chatMessages.appendChild(giftMessage);
                    });
                }
                else {
                    var userMessage = "Похожие товраы закончились";
                    var newTheirMessage = createMessageElement("message", userMessage, false);
                    chatMessages.appendChild(newTheirMessage);
                    document.getElementById("more-btn").style.display = "none";
                    return; // Завершаем выполнение функции, так как все подарки уже выведены
                }
            }
            scrollToBottom(chatMessages);
            document.getElementById("more-btn").style.display = "block";
        }
    });
}
function createGiftMessageElement(gift) {
    var giftMessage = document.createElement("div");
    giftMessage.classList.add("message", "gift-message");

    var avatar = document.createElement("div");
    avatar.classList.add("avatar");
    giftMessage.appendChild(avatar);

    var messageContent = document.createElement("div");
    messageContent.classList.add("message-content", "gift-content");

    var nameElement = document.createElement("p");
    nameElement.classList.add("gift-name");
    nameElement.textContent = "Название: " + gift.name;
    messageContent.appendChild(nameElement);

    var priceElement = document.createElement("p");
    priceElement.classList.add("gift-price");
    var priceValue = parseFloat(gift.price).toFixed(2).replace(/\.00$/, "");
    priceElement.textContent = "Цена: " + priceValue;
    messageContent.appendChild(priceElement);

    var traderElement = document.createElement("p");
    traderElement.classList.add("gift-trader");
    traderElement.textContent = "Торговец: " + gift.trader;
    messageContent.appendChild(traderElement);

    giftMessage.appendChild(messageContent);
    return giftMessage;
}

function sendMessage() {
    var chatMessages = document.querySelector(".chat-messages");
    var inputField = document.querySelector("input[name='text']");
    var userMessage = inputField.value;
    var newTheirMessage = createMessageElement("their-message", userMessage, true);
    chatMessages.appendChild(newTheirMessage);
    scrollToBottom(chatMessages);

    if (currentQuestionIndex > 0) {
        botDictionary[questions[currentQuestionIndex - 1]] = userMessage;
    }

    // Если все вопросы были заданы, отправляем ответы на сервер и получаем подарки
    if (currentQuestionIndex >= questions.length) {
        getGiftsFromServer();
    } else {
        var newMyMessage = createMessageElement("message", questions[currentQuestionIndex]);
        chatMessages.appendChild(newMyMessage);
        scrollToBottom(chatMessages);
        currentQuestionIndex++;
    }

    inputField.value = "";
}

document.getElementById("more-btn").addEventListener("click", function (e) {
    e.preventDefault();
    currentQuestionIndex = 0;
    var chatMessages = document.querySelector(".chat-messages");
    var inputField = document.querySelector("input[name='text']");
    var userMessage = "Еще";
    var newTheirMessage = createMessageElement("their-message", userMessage, true);
    chatMessages.appendChild(newTheirMessage);
    scrollToBottom(chatMessages);

    if (currentQuestionIndex > 0) {
        botDictionary[questions[currentQuestionIndex - 1]] = userMessage;
    }

    // Если все вопросы были заданы, отправляем ответы на сервер и получаем подарки
    if (currentQuestionIndex >= questions.length) {
        getGiftsFromServer();
    } else {
        var newMyMessage = createMessageElement("message", questions[currentQuestionIndex]);
        chatMessages.appendChild(newMyMessage);
        scrollToBottom(chatMessages);
        currentQuestionIndex++;
    }

    inputField.value = "";
});