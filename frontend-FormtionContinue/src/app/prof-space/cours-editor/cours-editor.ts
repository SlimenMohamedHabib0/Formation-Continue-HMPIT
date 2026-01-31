import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { Subscription, Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ProfCoursSrvc } from '../services/prof-cours-srvc';
import { ICours } from '../interfaces/icours';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

interface Icategory {
  id: number;
  libelle: string;
}

interface IprofLite {
  id: number;
  fullName: string;
  email: string;
}

interface ImeDto {
  id: string;
  fullName: string;
  email: string;
  role: string;
}

@Component({
  selector: 'app-cours-editor',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './cours-editor.html',
  styleUrl: './cours-editor.css',
})
export class CoursEditor implements OnInit, OnDestroy {
  loading = true;
  saving = false;
  error: string | null = null;
  success: string | null = null;

  isEdit = false;
  courseId: number | null = null;
  course: ICours | null = null;

  pdfUrl: SafeResourceUrl | null = null;
  private pdfObjectUrl: string | null = null;

  titre = '';
  description = '';
  motsCles = '';
  categoryName = '';

  loadingCats = false;
  categories: Icategory[] = [];

  coLoading = false;
  coError: string | null = null;
  coProfessors: IprofLite[] = [];

  profTerm = '';
  profSearching = false;
  profResults: IprofLite[] = [];

  currentUserId: number | null = null;

  private catSearch$ = new Subject<string>();
  private profSearch$ = new Subject<string>();
  private sub = new Subscription();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    private coursSrvc: ProfCoursSrvc,
    private sanitizer: DomSanitizer
  ) {}

  ngOnInit(): void {
    const idStr = this.route.snapshot.paramMap.get('id');
    this.isEdit = !!idStr;

    this.loadMe();

    this.sub.add(
      this.catSearch$.pipe(debounceTime(120), distinctUntilChanged()).subscribe((s) => {
        this.fetchCategories(s);
      })
    );

    this.sub.add(
      this.profSearch$.pipe(debounceTime(250), distinctUntilChanged()).subscribe((s) => {
        this.searchProfessors(s);
      })
    );

    if (this.isEdit) {
      const n = Number(idStr);
      this.courseId = Number.isFinite(n) ? n : null;

      if (!this.courseId) {
        this.loading = false;
        this.error = 'Identifiant du cours invalide.';
        return;
      }

      this.loadCourse(this.courseId);
      return;
    }

    this.loading = false;
    this.fetchCategories('');
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
    this.cleanupPdfUrl();
  }

  private cleanupPdfUrl(): void {
    if (this.pdfObjectUrl) {
      URL.revokeObjectURL(this.pdfObjectUrl);
      this.pdfObjectUrl = null;
    }
    this.pdfUrl = null;
  }

  private loadMe(): void {
    this.http.get<ImeDto>(`${environment.apiUrl}/Auth/me`).subscribe({
      next: (me) => {
        const n = Number(me?.id);
        this.currentUserId = Number.isFinite(n) ? n : null;
      },
      error: () => {
        this.currentUserId = null;
      },
    });
  }

  back(): void {
    this.router.navigate(['/prof/mes-cours']);
  }

  resetMessages(): void {
    this.error = null;
    this.success = null;
  }

  resetCoMessages(): void {
    this.coError = null;
  }

  get canEdit(): boolean {
    if (!this.isEdit) return true;
    return (this.course?.etat ?? '') === 'DRAFT';
  }

  loadCourse(id: number): void {
    this.loading = true;
    this.error = null;

    this.coursSrvc.getById(id).subscribe({
      next: (c) => {
        this.course = c;
        this.titre = c.titre ?? '';
        this.description = c.description ?? '';
        this.motsCles = c.motsCles ?? '';
        this.loading = false;

        this.fetchCategoryName(c.categoryId);
        this.fetchCategories('');
        this.buildPdfUrl();
        this.loadCoTeachers();
      },
      error: () => {
        this.loading = false;
        this.error = 'Impossible de charger le cours.';
      },
    });
  }

  fetchCategoryName(categoryId: number): void {
    this.http.get<Icategory>(`${environment.apiUrl}/categories/${categoryId}`).subscribe({
      next: (cat) => {
        if (!this.categoryName) this.categoryName = cat?.libelle ?? '';
      },
      error: () => {},
    });
  }

  onCategoryInput(): void {
    this.catSearch$.next(this.categoryName.trim());
  }

  fetchCategories(search: string): void {
    const s = search.trim();
    this.loadingCats = true;

    const url = s
      ? `${environment.apiUrl}/categories?search=${encodeURIComponent(s)}`
      : `${environment.apiUrl}/categories`;

    this.http.get<Icategory[]>(url).subscribe({
      next: (res) => {
        this.categories = res ?? [];
        this.loadingCats = false;
      },
      error: () => {
        this.categories = [];
        this.loadingCats = false;
      },
    });
  }

  badgeClass(etat: string): string {
    if (etat === 'PUBLISHED') return 'badge bg-green-lt';
    if (etat === 'DRAFT') return 'badge bg-orange-lt';
    return 'badge bg-secondary-lt';
  }

  save(): void {
    this.resetMessages();

    const titre = this.titre.trim();
    const description = (this.description ?? '').trim();
    const motsCles = this.motsCles.trim();
    const categoryName = this.categoryName.trim();

    if (!titre || !description || !motsCles || !categoryName) {
      this.error = 'Veuillez remplir tous les champs.';
      return;
    }

    if (this.isEdit && !this.courseId) {
      this.error = 'Identifiant du cours invalide.';
      return;
    }

    if (this.isEdit && !this.canEdit) {
      this.error = 'Ce cours est publié. Vous devez le repasser en brouillon pour modifier.';
      return;
    }

    this.saving = true;

    if (!this.isEdit) {
      this.coursSrvc.create({ titre, description, motsCles, categoryName }).subscribe({
        next: (created) => {
          this.saving = false;
          this.success = 'Cours créé.';
          this.router.navigate(['/prof/mes-cours'])
        },
        error: (e) => {
          this.saving = false;
          const msg = (e?.error ?? '').toString();
          this.error = msg ? msg : 'Impossible de créer le cours.';
        },
      });
      return;
    }

    this.coursSrvc.update(this.courseId!, { titre, description, motsCles, categoryName }).subscribe({
      next: () => {
        this.saving = false;
        this.success = 'Cours mis à jour.';
        this.loadCourse(this.courseId!);
          this.router.navigate(['/prof/mes-cours']);
      },
      error: (e) => {
        this.saving = false;
        const msg = (e?.error ?? '').toString();
        this.error = msg ? msg : 'Impossible de mettre à jour le cours.';
      },
    });
  }

  onPdfSelected(ev: Event): void {
    if (!this.courseId) return;
    if (!this.canEdit) return;

    const input = ev.target as HTMLInputElement;
    const file = input.files && input.files.length > 0 ? input.files[0] : null;
    input.value = '';
    if (!file) return;

    this.resetMessages();
    this.saving = true;

    this.coursSrvc.attachPdf(this.courseId, file).subscribe({
      next: () => {
        this.saving = false;
        this.success = 'PDF attaché.';
        this.loadCourse(this.courseId!);
      },
      error: (e) => {
        this.saving = false;
        const msg = (e?.error ?? '').toString();
        this.error = msg ? msg : 'Impossible d’attacher le PDF.';
      },
    });
  }

  buildPdfUrl(): void {
    this.cleanupPdfUrl();

    if (!this.courseId || !this.course?.nomFichierPdf) {
      this.pdfUrl = null;
      return;
    }

    this.coursSrvc.getPdfBlob(this.courseId).subscribe({
      next: (blob) => {
        this.pdfObjectUrl = URL.createObjectURL(blob);
        this.pdfUrl = this.sanitizer.bypassSecurityTrustResourceUrl(
          this.pdfObjectUrl + '#view=FitH&zoom=page-width'
        );
      },
      error: () => {
        this.pdfUrl = null;
      },
    });
  }

  loadCoTeachers(): void {
    if (!this.isEdit || !this.courseId) return;

    this.resetCoMessages();
    this.coLoading = true;

    this.coursSrvc.getCoProfessors(this.courseId).subscribe({
      next: (res) => {
        this.coProfessors = res ?? [];
        this.coLoading = false;
      },
      error: () => {
        this.coProfessors = [];
        this.coLoading = false;
        this.coError = 'Action impossible.';
      },
    });
  }

  onProfTermInput(): void {
    const t = this.profTerm.trim();
    if (!t) {
      this.profResults = [];
      return;
    }
    this.profSearch$.next(t);
  }

  searchProfessors(term: string): void {
    const t = term.trim();
    if (!t) {
      this.profResults = [];
      return;
    }

    this.resetCoMessages();
    this.profSearching = true;

    this.coursSrvc.searchProfessors(t).subscribe({
      next: (res) => {
        this.profResults = res ?? [];
        this.profSearching = false;
      },
      error: () => {
        this.profResults = [];
        this.profSearching = false;
        this.coError = 'Action impossible.';
      },
    });
  }

  alreadyAdded(p: IprofLite): boolean {
    return (this.coProfessors ?? []).some((x) => x.id === p.id);
  }

  isMe(p: IprofLite): boolean {
    return this.currentUserId !== null && p.id === this.currentUserId;
  }

  addCoTeacher(p: IprofLite): void {
    if (!this.courseId) return;
    if (!this.canEdit) return;
    if (this.alreadyAdded(p)) return;

    this.resetCoMessages();
    this.coLoading = true;

    this.coursSrvc.addCoProfessor(this.courseId, p.id).subscribe({
      next: () => {
        this.coLoading = false;
        this.profTerm = '';
        this.profResults = [];
        this.loadCoTeachers();
      },
      error: () => {
        this.coLoading = false;
        this.coError = 'Action impossible.';
      },
    });
  }

  removeCoTeacher(p: IprofLite): void {
    if (!this.courseId) return;
    if (!this.canEdit) return;
    if (this.isMe(p)) return;

    if (!confirm('Retirer ce professeur ?')) return;

    this.resetCoMessages();
    this.coLoading = true;

    this.coursSrvc.removeCoProfessor(this.courseId, p.id).subscribe({
      next: () => {
        this.coLoading = false;
        this.loadCoTeachers();
      },
      error: () => {
        this.coLoading = false;
        this.coError = 'Action impossible.';
      },
    });
  }
}
