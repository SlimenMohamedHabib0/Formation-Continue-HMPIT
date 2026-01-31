import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PasserTest } from './passer-test';

describe('PasserTest', () => {
  let component: PasserTest;
  let fixture: ComponentFixture<PasserTest>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PasserTest]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PasserTest);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
