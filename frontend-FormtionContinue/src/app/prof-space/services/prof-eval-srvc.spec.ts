import { TestBed } from '@angular/core/testing';

import { ProfEvalSrvc } from './prof-eval-srvc';

describe('ProfEvalSrvc', () => {
  let service: ProfEvalSrvc;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ProfEvalSrvc);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
