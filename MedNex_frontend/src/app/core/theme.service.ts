import { Injectable, inject, PLATFORM_ID, signal, computed } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

const THEME_KEY = 'theme';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private platformId = inject(PLATFORM_ID);

  // Signal is the source of truth — not the DOM.
  // Components read isDark() directly and react to changes automatically.
  private _isDark = signal<boolean>(true); // default: dark
  readonly isDark = this._isDark.asReadonly();
  readonly themeLabel = computed(() => (this._isDark() ? 'Dark' : 'Light'));

  // Call once in app.component.ts ngOnInit to apply the saved preference
  // before anything renders
  init(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    const saved = localStorage.getItem(THEME_KEY);
    const prefersDark = saved !== 'light'; // default to dark if nothing saved
    this._isDark.set(prefersDark);
    this.applyToDOM(prefersDark);
  }

  toggle(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    const next = !this._isDark();
    this._isDark.set(next);
    this.applyToDOM(next);
    localStorage.setItem(THEME_KEY, next ? 'dark' : 'light');
  }

  private applyToDOM(dark: boolean): void {
    document.body.classList.toggle('dark', dark);
  }
}