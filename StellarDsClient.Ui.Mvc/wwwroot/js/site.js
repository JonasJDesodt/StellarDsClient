﻿const body = document.querySelector('body');
const openOverlayButton = document.getElementById('open-overlay-button');
const closeOverlayButton = document.getElementById('close-overlay-button');

openOverlayButton.addEventListener('click', openOverlay);

//oAuthBaseAddress is a const set in _Layout.cshtml
if (document.referrer.startsWith('https://' + window.location.host) || document.referrer === oAuthBaseAddress) {
    const returnButton = document.getElementById('return-button');

    if (returnButton) {
        returnButton.addEventListener('click', () => window.history.back());
    }
}


function openOverlay() {
    body.classList.add('overlay');

    closeOverlayButton.addEventListener('click', closeOverlay);
    openOverlayButton.removeEventListener('click', openOverlay);
}

function closeOverlay() {
    body.classList.remove('overlay');

    closeOverlayButton.removeEventListener('click', closeOverlay);
    openOverlayButton.addEventListener('click', openOverlay);
}