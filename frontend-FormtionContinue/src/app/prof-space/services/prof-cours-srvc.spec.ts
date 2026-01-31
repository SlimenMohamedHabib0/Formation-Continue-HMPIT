import { TestBed } from '@angular/core/testing';

import { ProfCoursSrvc } from './prof-cours-srvc';

describe('ProfCoursSrvc', () => {
  let service: ProfCoursSrvc;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ProfCoursSrvc);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
