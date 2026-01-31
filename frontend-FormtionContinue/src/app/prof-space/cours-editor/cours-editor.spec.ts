import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CoursEditor } from './cours-editor';

describe('CoursEditor', () => {
  let component: CoursEditor;
  let fixture: ComponentFixture<CoursEditor>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CoursEditor]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CoursEditor);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
