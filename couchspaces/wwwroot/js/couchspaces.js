﻿async function pasteFromClipboard() {
    try {
        const text = await navigator.clipboard.readText();
        return text;
    } catch (err) {
        console.error('Failed to read clipboard contents: ', err);
        return '';
    }
}

function playSound() {
    var audio = new Audio('/sounds/start.mp3');
    audio.play();
}