import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { PublicSrvc } from '../public-srvc';

@Component({
  selector: 'app-lecture-cours',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './lecture-cours.html',
  styleUrl: './lecture-cours.css',
})
export class LectureCours implements OnInit, OnDestroy {
  loading = true;
  saving = false;

  error: string | null = null;
  success: string | null = null;

  courseId: number | null = null;

  pdfUrl: SafeResourceUrl | null = null;
  private pdfObjectUrl: string | null = null;

  completed = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private srvc: PublicSrvc,
    private sanitizer: DomSanitizer
  ) {}

  ngOnInit(): void {
    const n = Number(this.route.snapshot.paramMap.get('courseId'));
    this.courseId = Number.isFinite(n) ? n : null;

    if (!this.courseId) {
      this.loading = false;
      this.error = 'Identifiant du cours invalide.';
      return;
    }

    this.loadProgress();
    this.loadPdf();
  }

  ngOnDestroy(): void {
    if (this.pdfObjectUrl) URL.revokeObjectURL(this.pdfObjectUrl);
  }

  back(): void {
    this.router.navigate(['/public/mes-inscriptions']);
  }

  resetMsgs(): void {
    this.error = null;
    this.success = null;
  }

  private loadProgress(): void {
    if (!this.courseId) return;

    this.srvc.getProgress(this.courseId).subscribe({
      next: (p: any) => {
        this.completed = !!p?.dateCompletion;
      },
      error: () => {
        this.completed = false;
      },
    });
  }

  private loadPdf(): void {
    this.loading = true;
    this.error = null;

    this.srvc.getCoursePdfBlob(this.courseId!).subscribe({
      next: (blob: Blob) => {
        try {
          if (this.pdfObjectUrl) URL.revokeObjectURL(this.pdfObjectUrl);

          this.pdfObjectUrl = URL.createObjectURL(blob);
          this.pdfUrl = this.sanitizer.bypassSecurityTrustResourceUrl(
            this.pdfObjectUrl + '#view=FitH&zoom=page-width'
          );

          this.loading = false;
        } catch {
          this.loading = false;
          this.error = 'Impossible de charger le PDF.';
        }
      },
      error: () => {
        this.loading = false;
        this.error = 'Impossible de charger le PDF.';
      },
    });
  }

  finish(): void {
    if (!this.courseId) return;
    if (this.completed) return;

    this.resetMsgs();
    this.saving = true;
    this.srvc.updateProgress(this.courseId, 1, 1).subscribe({
      next: () => {
        this.saving = false;
        this.completed = true;
        this.success = 'Cours marqué comme terminé.';
      },
      error: (e: any) => {
        this.saving = false;
        const msg = (e?.error ?? '').toString();
        this.error = msg ? msg : 'Action impossible.';
      },
    });
  }

  goTest(): void {
    if (!this.courseId) return;
    this.router.navigate(['/public/passer-test', this.courseId]);
  }
}
