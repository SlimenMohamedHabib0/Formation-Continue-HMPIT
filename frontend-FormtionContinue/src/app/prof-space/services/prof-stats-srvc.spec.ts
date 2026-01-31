import { TestBed } from '@angular/core/testing';

import { ProfStatsSrvc } from './prof-stats-srvc';

describe('ProfStatsSrvc', () => {
  let service: ProfStatsSrvc;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ProfStatsSrvc);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
