
export const validateName = (name) => name.length >= 4 && name.length <= 20;
export const validateDescription = (desc) => desc.length <= 256;
export const validateEmail = (email) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
export const validatePhone = (phone) => /^[89]\d{7}$/.test(phone);
