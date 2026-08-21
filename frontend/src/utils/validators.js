export function isValidEmail(email) {
    const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return regex.test(email);
}

export function isValidPassword(password) {
    return password.length >= 6;
}

export function isValidName(name) {
    return name.trim().length >= 2;
}