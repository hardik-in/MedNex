import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export type ToastType = 'success' | 'error' | 'warning' | 'info' | 'danger';

export interface Toast {
  id: number;
  message: string;
  type: ToastType;
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  private counter = 0;
  private toastsSubject = new BehaviorSubject<Toast[]>([]);
  toasts$ = this.toastsSubject.asObservable();

  show(message: string, type: ToastType = 'info', duration = 3500) {
    const id = ++this.counter;
    const current = this.toastsSubject.getValue();
    this.toastsSubject.next([...current, { id, message, type }]);
    setTimeout(() => this.dismiss(id), duration);
  }

  dismiss(id: number) {
    this.toastsSubject.next(
      this.toastsSubject.getValue().filter((t) => t.id !== id),
    );
  }

  success(message: string) {
    this.show(message, 'success');
  }
  error(message: string) {
    this.show(message, 'error');
  }
  warning(message: string) {
    this.show(message, 'warning');
  }
  info(message: string) {
    this.show(message, 'info');
  }
  danger(message: string) {
    this.show(message, 'danger');
  }
}
