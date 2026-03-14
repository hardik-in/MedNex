import { TestBed } from '@angular/core/testing';

import { AdminAppointmentsService } from './admin-appointments.service';

describe('AdminAppointmentsService', () => {
  let service: AdminAppointmentsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AdminAppointmentsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
