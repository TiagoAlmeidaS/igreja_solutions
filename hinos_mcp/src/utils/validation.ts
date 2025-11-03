/**
 * Validation utilities
 */

export function validateEmail(email: string): boolean {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email);
}

export function validateRequired(value: any, fieldName: string): void {
  if (value === undefined || value === null || value === '') {
    throw new Error(`${fieldName} é obrigatório`);
  }
}

export function validateNumber(value: any, fieldName: string): number {
  const num = Number(value);
  if (isNaN(num)) {
    throw new Error(`${fieldName} deve ser um número válido`);
  }
  return num;
}

export function validatePositiveNumber(value: any, fieldName: string): number {
  const num = validateNumber(value, fieldName);
  if (num <= 0) {
    throw new Error(`${fieldName} deve ser um número positivo`);
  }
  return num;
}

