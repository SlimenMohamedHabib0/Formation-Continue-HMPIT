import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, Subscription, debounceTime, distinctUntilChanged } from 'rxjs';
import { PublicSrvc } from '../public-srvc';

interface ICourseCard {
  id: number;
  titre: string;
  description: string;
  motsCles: string;
  datePublication: string;
  categoryId: number;
  categoryLibelle: string;
  nomFichierPdf: string | null;
}

type EnrollmentStatut = 'PENDING' | 'ACCEPTEE' | 'REFUSEE';

@Component({
  selector: 'app-cours-public',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './cours.html',
  styleUrl: './cours.css',
})
export class Cours implements OnInit, OnDestroy {
  loadingPage = true;     
   loadingCourses = false; 
  error: string | null = null;

  search = '';
  courses: ICourseCard[] = [];

  enrollByCourseId = new Map<number, EnrollmentStatut>();
  rowLoading = new Map<number, boolean>();

  private search$ = new Subject<string>();
  private sub = new Subscription();

  constructor(private srvc: PublicSrvc) {}

  ngOnInit(): void {
    this.sub.add(
      this.search$.pipe(debounceTime(100), distinctUntilChanged()).subscribe(() => {
        this.loadCourses(); 
      })
    );

    this.loadAll();
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  loadAll(): void {
    this.error = null;
    this.loadingPage = true;

    this.loadMyEnrollments();
    this.loadCourses(true);
  }

  loadCourses(isFirstLoad = false): void {
    this.error = null;

    if (isFirstLoad) this.loadingPage = true;
    this.loadingCourses = true;

    const s = this.search.trim();

    this.srvc.getPublishedCourses(s || undefined).subscribe({
      next: (res) => {
        this.courses = (res ?? []) as ICourseCard[];
        this.loadingCourses = false;
        this.loadingPage = false;
      },
      error: () => {
        this.courses = [];
        this.loadingCourses = false;
        this.loadingPage = false;
        this.error = 'Impossible de charger les cours.';
      },
    });
  }

  loadMyEnrollments(): void {
    this.srvc.getMyEnrollments().subscribe({
      next: (res) => {
        const list = (res ?? []) as any[];
        this.enrollByCourseId.clear();

        for (const e of list) {
          const cid = Number(e.courseId ?? e.CourseId);
          const st = (e.statut ?? e.Statut ?? '') as EnrollmentStatut;
          if (Number.isFinite(cid) && st) this.enrollByCourseId.set(cid, st);
        }
      },
      error: () => {
        this.enrollByCourseId.clear();
      },
    });
  }

  onSearchChange(): void {
    this.search$.next(this.search);
  }

  getStatus(courseId: number): EnrollmentStatut | null {
    return this.enrollByCourseId.get(courseId) ?? null;
  }

  canRequest(courseId: number): boolean {
    return this.getStatus(courseId) === null;
  }

  statusBadge(st: EnrollmentStatut): string {
    if (st === 'PENDING') return 'badge bg-yellow-lt';
    if (st === 'ACCEPTEE') return 'badge bg-green-lt';
    return 'badge bg-red-lt';
  }

  statusLabel(st: EnrollmentStatut): string {
    if (st === 'PENDING') return 'En attente';
    if (st === 'ACCEPTEE') return 'Acceptée';
    return 'Refusée';
  }

  request(courseId: number): void {
    if (!this.canRequest(courseId)) return;

    this.rowLoading.set(courseId, true);
    this.error = null;

    this.srvc.requestEnrollment(courseId).subscribe({
      next: () => {
        this.rowLoading.set(courseId, false);
        this.enrollByCourseId.set(courseId, 'PENDING');
      },
      error: (e) => {
        this.rowLoading.set(courseId, false);
        const msg = (e?.error ?? '').toString();
        this.error = msg ? msg : 'Action impossible.';
      },
    });
  }

  isRowLoading(courseId: number): boolean {
    return this.rowLoading.get(courseId) === true;
  }

  keywords(motsCles: string): string[] {
    return (motsCles ?? '')
      .split(',')
      .map((x) => x.trim())
      .filter((x) => !!x);
  }
}
