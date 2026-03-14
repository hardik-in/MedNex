import { Injectable, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private platformId = inject(PLATFORM_ID);
  private THEME_KEY = 'theme';

  init() {
    if (!isPlatformBrowser(this.platformId)) return;
    const saved = localStorage.getItem(this.THEME_KEY);
    if (saved === 'light') {
      document.body.classList.remove('dark');
    } else {
      this.applyDark();
    }
  }

  toggle() {
    if (!isPlatformBrowser(this.platformId)) return;
    const isDark = document.body.classList.contains('dark');
    if (isDark) {
      document.body.classList.remove('dark');
      localStorage.setItem(this.THEME_KEY, 'light');
    } else {
      document.body.classList.add('dark');
      localStorage.setItem(this.THEME_KEY, 'dark');
    }
  }

  isDark(): boolean {
    if (!isPlatformBrowser(this.platformId)) return false;
    return document.body.classList.contains('dark');
  }

  private applyDark() {
    document.body.classList.add('dark');
  }
}
