import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, Subscription, debounceTime, distinctUntilChanged } from 'rxjs';
import { AdminSrvc } from '../admin-srvc';
import { Icours } from '../interfaces/icours';
import { Icategory } from '../interfaces/icategory';
import { Ienrollment } from '../interfaces/ienrollment';
import { Iattempt } from '../interfaces/iattempt';

type ViewTab = 'courses' | 'enrollments' | 'results';
type CourseTab = 'published' | 'draft';

@Component({
  selector: 'app-supervision',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './supervision.html',
  styleUrls: ['./supervision.css'],
})
export class Supervision implements OnInit, OnDestroy {
  viewTab: ViewTab = 'courses';

  categories: Icategory[] = [];
  allCategories: Icategory[] = [];

  categorySearch = '';
  selectedCategoryId: number | null = null;
  loadingCats = true;

  loading = true;
  error: string | null = null;
  success: string | null = null;

  courses: Icours[] = [];
  courseTab: CourseTab = 'published';
  courseSearch = '';

  enrollments: Ienrollment[] = [];
  enrollmentSearch = '';
  enrollmentStatut: string = '';

  attempts: Iattempt[] = [];
  attemptSearch = '';
  attemptStatut: string = '';

  private catsSearch$ = new Subject<string>();
  private coursesFilter$ = new Subject<string>();
  private enrollmentsFilter$ = new Subject<string>();
  private attemptsFilter$ = new Subject<string>();
  private sub = new Subscription();

  constructor(private adminSrvc: AdminSrvc) {}

  ngOnInit(): void {
    this.loadCategories('');
    this.loadCourses();

    this.sub.add(
      this.catsSearch$
        .pipe(debounceTime(10), distinctUntilChanged())
        .subscribe((s) => this.loadCategories(s))
    );

    this.sub.add(
      this.coursesFilter$
        .pipe(debounceTime(10), distinctUntilChanged())
        .subscribe(() => this.loadCourses())
    );

    this.sub.add(
      this.enrollmentsFilter$
        .pipe(debounceTime(10), distinctUntilChanged())
        .subscribe(() => this.loadEnrollments())
    );

    this.sub.add(
      this.attemptsFilter$
        .pipe(debounceTime(10), distinctUntilChanged())
        .subscribe(() => this.loadAttempts())
    );
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  setViewTab(t: ViewTab): void {
    this.resetMessages();
    this.viewTab = t;

    if (t === 'courses') this.triggerCoursesReload();
    if (t === 'enrollments') this.triggerEnrollmentsReload();
    if (t === 'results') this.triggerAttemptsReload();
  }

  setCourseTab(t: CourseTab): void {
    this.courseTab = t;
    this.triggerCoursesReload();
  }

  onCategorySearchChange(): void {
    this.catsSearch$.next(this.categorySearch.trim());
  }

  onCategorySelectChange(): void {
    if (this.viewTab === 'courses') this.triggerCoursesReload();
    if (this.viewTab === 'enrollments') this.triggerEnrollmentsReload();
    if (this.viewTab === 'results') this.triggerAttemptsReload();
  }

  resetMessages(): void {
    this.error = null;
    this.success = null;
  }

  loadCategories(search: string): void {
    this.loadingCats = true;

    this.adminSrvc.getCategories(search).subscribe({
      next: (res) => {
        this.categories = res ?? [];
        if (!search) {
          this.allCategories = this.categories;
        }
        this.loadingCats = false;
      },
      error: () => {
        this.categories = [];
        if (!search) this.allCategories = [];
        this.loadingCats = false;
      },
    });
  }

  categoryLabelById(id: number): string {
    const found = this.allCategories.find((x) => x.id === id);
    return found ? found.libelle : `#${id}`;
  }

  loadCourses(): void {
    this.loading = true;
    this.error = null;

    const etat = this.courseTab === 'published' ? 'PUBLISHED' : 'DRAFT';
    const s = this.courseSearch.trim() ? this.courseSearch.trim() : undefined;
    const cat = this.selectedCategoryId ?? undefined;

    this.adminSrvc.getCourses(s, cat, etat).subscribe({
      next: (res) => {
        this.courses = res ?? [];
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.error = 'Impossible de charger la liste des cours.';
      },
    });
  }

  onCoursesFiltersChange(): void {
    this.triggerCoursesReload();
  }

  private triggerCoursesReload(): void {
    const key =
      `${this.viewTab}|${this.courseTab}|` +
      `${this.courseSearch.trim().toLowerCase()}|` +
      `${this.selectedCategoryId ?? ''}`;
    this.coursesFilter$.next(key);
  }

  unpublish(c: Icours): void {
    this.resetMessages();
    if (!confirm('Retirer ce cours de la publication ?')) return;

    this.loading = true;
    this.adminSrvc.unpublishCourse(c.id).subscribe({
      next: () => {
        this.loading = false;
        this.success = 'Cours dépublié.';
        this.triggerCoursesReload();
      },
      error: () => {
        this.loading = false;
        this.error = 'Impossible de dépublier ce cours.';
      },
    });
  }

  deleteDraft(c: Icours): void {
    this.resetMessages();
    if (!confirm('Supprimer ce brouillon ?')) return;

    this.loading = true;
    this.adminSrvc.deleteDraftCourse(c.id).subscribe({
      next: () => {
        this.loading = false;
        this.success = 'Brouillon supprimé.';
        this.triggerCoursesReload();
      },
      error: () => {
        this.loading = false;
        this.error = 'Impossible de supprimer ce brouillon.';
      },
    });
  }

  profLabel(c: Icours): string {
    if (!c.professors || c.professors.length === 0) return '-';
    if (c.professors.length === 1) return c.professors[0].fullName;
    return `${c.professors[0].fullName} +${c.professors.length - 1}`;
  }

  loadEnrollments(): void {
    this.loading = true;
    this.error = null;

    const s = this.enrollmentSearch.trim() ? this.enrollmentSearch.trim() : undefined;
    const st = this.enrollmentStatut.trim() ? this.enrollmentStatut.trim() : undefined;
    const cat = this.selectedCategoryId ?? undefined;

    this.adminSrvc.getEnrollments(s, st, undefined, cat).subscribe({
      next: (res) => {
        this.enrollments = res ?? [];
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.error = 'Impossible de charger la liste des inscriptions.';
      },
    });
  }

  onEnrollmentsFiltersChange(): void {
    this.triggerEnrollmentsReload();
  }

  private triggerEnrollmentsReload(): void {
    const key =
      `${this.viewTab}|` +
      `${this.enrollmentSearch.trim().toLowerCase()}|` +
      `${this.enrollmentStatut}|` +
      `${this.selectedCategoryId ?? ''}`;
    this.enrollmentsFilter$.next(key);
  }

  loadAttempts(): void {
    this.loading = true;
    this.error = null;

    const s = this.attemptSearch.trim() ? this.attemptSearch.trim() : undefined;
    const st = this.attemptStatut.trim() ? this.attemptStatut.trim() : undefined;
    const cat = this.selectedCategoryId ?? undefined;

    this.adminSrvc.getAttempts(s, st, undefined, cat).subscribe({
      next: (res) => {
        this.attempts = res ?? [];
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.error = 'Impossible de charger la liste des résultats.';
      },
    });
  }

  onAttemptsFiltersChange(): void {
    this.triggerAttemptsReload();
  }

  private triggerAttemptsReload(): void {
    const key =
      `${this.viewTab}|` +
      `${this.attemptSearch.trim().toLowerCase()}|` +
      `${this.attemptStatut}|` +
      `${this.selectedCategoryId ?? ''}`;
    this.attemptsFilter$.next(key);
  }

  resetAllFilters(): void {
    this.resetMessages();

    this.selectedCategoryId = null;
    this.categorySearch = '';
    this.loadCategories('');

    this.courseSearch = '';
    this.courseTab = 'published';

    this.enrollmentSearch = '';
    this.enrollmentStatut = '';

    this.attemptSearch = '';
    this.attemptStatut = '';

    if (this.viewTab === 'courses') this.triggerCoursesReload();
    if (this.viewTab === 'enrollments') this.triggerEnrollmentsReload();
    if (this.viewTab === 'results') this.triggerAttemptsReload();
  }
}
