import { TestBed } from '@angular/core/testing';

import { PublicSrvc } from './public-srvc';

describe('PublicSrvc', () => {
  let service: PublicSrvc;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PublicSrvc);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
