const userList = document.querySelectorAll('.user-list li');

userList.forEach(li => {
    li.addEventListener('click', async () => {
        userList.forEach(item => item.classList.remove('active'));
        li.classList.add('active');

        userName = li.textContent.trim();
        getUserAllInfo(userName);
    });
});
function getUserAllInfo(userName) {
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
                displayUserAllInfo(user);
            }
        })
        .catch(error => {
            console.error('Error fetching user information:', error);
        });
}
function displayUserAllInfo(user) {
    // Заполняем значения в полях ввода
    document.getElementById('login').value = user.логин;
    document.getElementById('password').value = user.пароль;
    document.getElementById('survival').value = user.выживания || 0;
    document.getElementById('death').value = user.смерти || 0;
    document.getElementById('lost').value = user.потерянБезвести || 0;
    document.getElementById('count').value = user.количествоРейдов || 0;
    document.getElementById('murders').value = user.убийства || 0;
    document.getElementById('murdersChVK').value = user.убийстваЧвк || 0;

    const imageElement = document.getElementById('image_user');
    if (user.доказательство) {
        imageElement.src = `data:image/jpeg;base64,${user.доказательство}`;
        imageElement.style.display = 'block'; // Показываем изображение
    } else {
        imageElement.src = ''; // Очищаем источник изображения
        imageElement.style.display = 'none'; // Скрываем изображение
    }
}

document.getElementById('approve-btn').addEventListener('click', function (event) {
    event.preventDefault();
    const activeUserName = document.querySelector('.user-list li.active').textContent.trim();
    const userData = {
        логин: activeUserName,
        выживания: document.getElementById('survival').value,
        смерти: document.getElementById('death').value,
        потерянБезвести: document.getElementById('lost').value,
        количествоРейдов: document.getElementById('count').value,
        убийства: document.getElementById('murders').value,
        убийстваЧвк: document.getElementById('murdersChVK').value,
        одобрено: true
    };

    fetch('/SaveChangesModer', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(userData)
    })
        .then(response => {
            if (response.ok) {
                console.log('Данные успешно сохранены');
            } else {
                console.error('Ошибка при сохранении данных');
            }
        })
        .catch(error => {
            console.error('Ошибка при отправке запроса:', error);
        });
});