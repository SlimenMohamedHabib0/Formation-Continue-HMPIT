import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Professeurs } from './professeurs';

describe('Professeurs', () => {
  let component: Professeurs;
  let fixture: ComponentFixture<Professeurs>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Professeurs]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Professeurs);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
