import { TestBed } from '@angular/core/testing';

import { AdminPatientsServiceTsService } from './admin-patients.service.ts.service';

describe('AdminPatientsServiceTsService', () => {
  let service: AdminPatientsServiceTsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AdminPatientsServiceTsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
