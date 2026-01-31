import { TestBed } from '@angular/core/testing';

import { AdminSrvc } from './admin-srvc';

describe('AdminSrvc', () => {
  let service: AdminSrvc;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AdminSrvc);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
