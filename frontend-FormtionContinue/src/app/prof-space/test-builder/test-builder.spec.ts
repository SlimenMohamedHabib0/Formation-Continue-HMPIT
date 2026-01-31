import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TestBuilder } from './test-builder';

describe('TestBuilder', () => {
  let component: TestBuilder;
  let fixture: ComponentFixture<TestBuilder>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TestBuilder]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TestBuilder);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
