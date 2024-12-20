﻿const photoUploader = document.getElementById('photo-uploader');

photoUploader.addEventListener('change', (event) => {
    event.preventDefault();
    const file = photoUploader.files[0];
    var fileExtension = ['jpg', 'jpeg', 'png'];
    var ext = file.name.split('.').pop().toLowerCase();
    var isPhoto = false;
    fileExtension.forEach(function (elem) {
        if (elem == ext) {
            isPhoto = true;
            console.log(isPhoto);
        }
        console.log(isPhoto);
    }
    )
    if (isPhoto === false) {
        photoUploader.value = "";
        const gameImage = document.getElementById('uploadingGameImage');
        gameImage.src = "";
    }
    else {
        const gameImage = document.getElementById('uploadingGameImage');
        var fileReader = new FileReader();
        fileReader.onload = function () {
            gameImage.src = fileReader.result;
        }
        fileReader.readAsDataURL(photoUploader.files[0]);
    }
});
