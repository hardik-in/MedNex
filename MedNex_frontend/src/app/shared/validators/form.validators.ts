import { AbstractControl, ValidationErrors } from '@angular/forms';

/**
 * Validates password complexity: requires uppercase, number, and special character.
 * Used on both register-patient and register-admin forms.
 */
export function passwordComplexityValidator(
  control: AbstractControl,
): ValidationErrors | null {
  const val = control.value ?? '';
  if (!val) return null;
  const hasUpper   = /[A-Z]/.test(val);
  const hasNumber  = /[0-9]/.test(val);
  const hasSpecial = /[^A-Za-z0-9]/.test(val);
  return hasUpper && hasNumber && hasSpecial ? null : { complexity: true };
}

/**
 * Cross-field validator: confirms password and confirmPassword match.
 * Applied at the FormGroup level, not the control level.
 */
export function passwordMatchValidator(
  group: AbstractControl,
): ValidationErrors | null {
  const pw  = group.get('password')?.value;
  const cpw = group.get('confirmPassword')?.value;
  return pw && cpw && pw !== cpw ? { mismatch: true } : null;
}

/**
 * Factory: returns a validator that enforces a minimum age based on date of birth.
 * @param minAge - Minimum age in years
 */
export function minAgeValidator(minAge: number) {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) return null;
    const dob   = new Date(control.value);
    const today = new Date();
    const age   =
      today.getFullYear() -
      dob.getFullYear() -
      (today < new Date(today.getFullYear(), dob.getMonth(), dob.getDate())
        ? 1
        : 0);
    return age >= minAge ? null : { minAge: true };
  };
}