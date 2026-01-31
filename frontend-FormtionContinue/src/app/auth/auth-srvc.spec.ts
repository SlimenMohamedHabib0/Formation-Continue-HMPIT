import { TestBed } from '@angular/core/testing';

import { AuthSrvc } from './auth-srvc';

describe('AuthSrvc', () => {
  let service: AuthSrvc;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AuthSrvc);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
