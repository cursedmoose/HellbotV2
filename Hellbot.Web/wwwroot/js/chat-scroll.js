export function isNearBottom(element, thresholdPx = 80) {
    if (!element) return true;
    return element.scrollHeight - element.scrollTop - element.clientHeight <= thresholdPx;
}

export function scrollToBottom(element) {
    if (!element) return;
    element.scrollTop = element.scrollHeight;
}
