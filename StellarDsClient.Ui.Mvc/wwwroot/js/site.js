const body = document.querySelector('body');
const openOverlayButton = document.getElementById('open-overlay-button');
const closeOverlayButton = document.getElementById('close-overlay-button');

openOverlayButton.addEventListener('click', openOverlay);

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