const chatContainer = document.getElementById('user-filters');

document.getElementById('toggle-btn').addEventListener('click', function (event) {
    event.preventDefault();
    chatContainer.classList.toggle('hidden');
});

const userList = document.querySelectorAll('.user-list li');

userList.forEach(li => {
    li.addEventListener('click', async () => {
        userList.forEach(item => item.classList.remove('active'));
        li.classList.add('active');

        userName = li.textContent.trim();
        getUserInfo(userName);
        const chatMessages = document.querySelector(".chat-messages");
        while (chatMessages.firstChild) {
            chatMessages.removeChild(chatMessages.firstChild);
        }

        try {
            await getMessagesFromServer(userName);
        } catch (error) {
            console.error('Ошибка при получении сообщений:', error);
        }
    });
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
var totalDisplayedGifts = 0;
function getMessagesFromServer(userName) {
    var chatMessages = document.querySelector(".chat-messages");
    $.ajax({
        type: "POST",
        url: "/GetMessages",
        contentType: "application/json",
        data: JSON.stringify(userName),
        success: function (data) {
            if (data.totalCount === 0) {
            }
            else {
                if (data.messages.length === 0) {
                    const userMessage = "У вас нет новых сообщений";
                    const newTheirMessage = createMessageElement("message", userMessage, false);
                    chatMessages.appendChild(newTheirMessage);
                } else {
                    data.messages.forEach(message => {
                        if (message.позиция === "left") {
                            const newMessage = createMessageElement("their-message", message.сообщение, true);
                            chatMessages.appendChild(newMessage);
                        }
                        else {
                            const newMessage = createMessageElement("message", message.сообщение, false);
                            chatMessages.appendChild(newMessage);
                        }
                    });
                }
            }
            scrollToBottom(chatMessages);
        }
    });
}

function createMessageElement(className, content, isTheir = false) {
    var messageElement = document.createElement("div");
    messageElement.className = "message " + className;
    messageElement.innerHTML = `<div class='avatar ${isTheir ? "their-avatar" : ""}'></div><div class='message-content ${isTheir ? "their" : ""}'>${content}</div>`;
    return messageElement;
};

const chatMessages = document.querySelector(".chat-messages");
const chatInput = document.getElementById("chat-input");

document.getElementById("send-button").addEventListener("click", function (event) {
    event.preventDefault();
    const message = chatInput.value.trim();
    if (message) {
        const currentDate = new Date();
        const formattedDate = currentDate;
        const recipient = userName; // Replace with the actual recipient's login

        // Create the message element
        const messageElement = document.createElement("div");
        messageElement.classList.add("message", "their-message");

        const avatarElement = document.createElement("div");
        avatarElement.classList.add("avatar", "their-avatar");

        const messageContentElement = document.createElement("div");
        messageContentElement.classList.add("message-content", "their");
        messageContentElement.textContent = message;

        messageElement.appendChild(avatarElement);
        messageElement.appendChild(messageContentElement);

        chatMessages.appendChild(messageElement);

        chatInput.value = "";
        sendMessageToServer(formattedDate, recipient, message);
    }
});

function sendMessageToServer(dateTime, recipient, message) {
    const messageData = {
        userName: recipient,
        formattedDate: dateTime,
        message: message
    };
    $.ajax({
        type: "POST",
        url: "/AddMessages",
        contentType: "application/json",
        data: JSON.stringify(messageData),
        success: function (data) {
            if (data.error) {
                console.error("Error sending message:", data.error);
            } else {
                console.log("Message sent successfully");
            }
        },
        error: function (xhr, status, error) {
            console.error("Error sending message:", error);
        }
    });
}

function getUserInfo(userName) {
    fetch('/GetInformation', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(userName)
    })
        .then(response => response.json())
        .then(user => {
            if (user.error) {
                // Обработка ошибки
                console.error(user.error);
            } else {
                displayUserInfo(user);
            }
        })
        .catch(error => {
            console.error('Error fetching user information:', error);
        });
}

function displayUserInfo(user) {
    document.getElementById('user-survivals').textContent = user.выживания || 0;
    document.getElementById('user-deaths').textContent = user.смерти || 0;
    document.getElementById('user-missing').textContent = user.потерянБезвести || 0;
    document.getElementById('user-raids').textContent = user.количествоРейдов || 0;
    document.getElementById('user-kills').textContent = user.убийства || 0;
    document.getElementById('user-kills-cvk').textContent = user.убийстваЧвк || 0;
}
// Получаем элементы, с которыми будем работать
const userFilterSelect = document.getElementById('user-filter-select');
const user_list = document.getElementById('user-list');

// Добавляем обработчик события на изменение значения select
userFilterSelect.addEventListener('change', (event) => {
    event.preventDefault();
    const selectedCardName = userFilterSelect.value;
    const data = {
        selectedCardName: selectedCardName
    };
    user_list.innerHTML = '';
    fetch('/ChangeCard', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    })
        .then(response => response.json())
        .then(data => {
            //const user_list = $('#user-list');
            //user_list.empty(); // Очищаем список пользователей перед добавлением новых

            //// Добавляем новых пользователей в список
            //data.forEach(user => {
            //    const li = $('<li>').text(user.логин);
            //    user_list.append(li);
            //});
        })
        .catch(error => {
            console.error('Error fetching user information:', error);
        });

});