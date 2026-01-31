import { Component, OnDestroy, OnInit } from '@angular/core'
import { CommonModule } from '@angular/common'
import { FormsModule } from '@angular/forms'
import { Router, ActivatedRoute } from '@angular/router'
import { HttpClient } from '@angular/common/http'
import { Subject, Subscription, debounceTime, distinctUntilChanged } from 'rxjs'
import { ProfCoursSrvc } from '../services/prof-cours-srvc'
import { ICours } from '../interfaces/icours'
import { Icategory } from '../interfaces/icategory'
import { environment } from '../../../environments/environment'

type EtatFilter = '' | 'PUBLISHED' | 'DRAFT'
type RowAction = 'publish' | 'unpublish' | 'delete'

@Component({
  selector: 'app-mes-cours',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './mes-cours.html',
  styleUrls: ['./mes-cours.css'],
})
export class MesCours implements OnInit, OnDestroy {
  loading = true
  loadingCats = true

  error: string | null = null
  actionError: string | null = null

  actionLoading: Record<number, Partial<Record<RowAction, boolean>>> = {}

  courses: ICours[] = []
  categories: Icategory[] = []

  search = ''
  selectedEtat: EtatFilter = ''
  selectedCategoryId: number | null = null

  private filter$ = new Subject<string>()
  private sub = new Subscription()

  constructor(
    private profCoursSrvc: ProfCoursSrvc,
    private http: HttpClient,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.loadCategories()
    this.loadCourses()

    this.sub.add(
      this.filter$.pipe(debounceTime(150), distinctUntilChanged()).subscribe(() => this.loadCourses())
    )
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe()
  }

  private extractBackendMessage(e: any): string {
    const raw = e?.error

    if (typeof raw === 'string') return raw.trim()

    if (raw && typeof raw === 'object') {
      const m = raw?.message ?? raw?.Message ?? raw?.title ?? raw?.Title
      if (typeof m === 'string' && m.trim()) return m.trim()

      const errors = raw?.errors
      if (errors && typeof errors === 'object') {
        const firstKey = Object.keys(errors)[0]
        const arr = (errors as any)[firstKey]
        if (Array.isArray(arr) && arr[0]) return String(arr[0]).trim()
      }
    }

    return ''
  }

  private friendly(action: RowAction | 'loadCourses', e: any): string {
    const status = e?.status
    const msg = this.extractBackendMessage(e)
    const lower = msg.toLowerCase()

    if (status === 0) return 'Connexion interrompue. Réessayez.'
    if (status === 401) return 'Session expirée. Veuillez vous reconnecter.'
    if (status === 403) return 'Accès refusé.'
    if (status === 404) return 'Ressource introuvable.'

    if (lower.includes('you must attach a pdf'))
      return 'Attachez un PDF avant de publier.'

    if (lower.includes('qcm invalid'))
      return 'Impossible de publier : QCM invalide (total = 20 + choix valides).'

    if (lower.includes('only draft courses can be published'))
      return 'Publication impossible : le cours doit être en brouillon.'

    if (lower.includes('only published courses can be unpublished'))
      return 'Dépublication impossible : le cours doit être publié.'

    if (lower.includes('cannot delete course') && lower.includes('attempts exist'))
      return 'Suppression impossible : des tentatives existent déjà.'

    if (msg) return msg

    if (action === 'loadCourses') return 'Impossible de charger vos cours.'
    if (action === 'publish') return 'Impossible de publier le cours.'
    if (action === 'unpublish') return 'Impossible de dépublier le cours.'
    return 'Impossible de supprimer le cours.'
  }

  onFiltersChange(): void {
    const key = `${this.search.trim().toLowerCase()}|${this.selectedEtat}|${this.selectedCategoryId ?? ''}`
    this.filter$.next(key)
  }

  _toggleFilters(): void {
    this.search = ''
    this.selectedEtat = ''
    this.selectedCategoryId = null
    this.onFiltersChange()
  }

  loadCategories(): void {
    this.loadingCats = true

    this.http.get<Icategory[]>(`${environment.apiUrl}/categories`).subscribe({
      next: (res) => {
        this.categories = res ?? []
        this.loadingCats = false
      },
      error: () => {
        this.categories = []
        this.loadingCats = false
      },
    })
  }

  categoryLabelById(id: number): string {
    const found = this.categories.find((c) => c.id === id)
    return found ? found.libelle : ''
  }

  loadCourses(): void {
    this.loading = true
    this.error = null

    const s = this.search.trim() || undefined
    const cat = this.selectedCategoryId ?? undefined

    this.profCoursSrvc.getCourses(s, cat).subscribe({
      next: (res) => {
        this.courses = res ?? []
        this.loading = false
      },
      error: (e) => {
        this.loading = false
        this.error = this.friendly('loadCourses', e)
      },
    })
  }

  get filteredCourses(): ICours[] {
    if (!this.selectedEtat) return this.courses
    return this.courses.filter((c) => c.etat === this.selectedEtat)
  }

  goCreate(): void {
    this.router.navigate(['/prof/cours-editor'])
  }

  goEdit(c: ICours): void {
    this.router.navigate(['/prof/cours-editor', c.id])
  }

  goTest(c: ICours): void {
    this.router.navigate(['/prof/test-builder', c.id])
  }

  canPublish(c: ICours): boolean {
    return c.etat === 'DRAFT' && !!c.nomFichierPdf
  }

  isActionLoading(c: ICours, action: RowAction): boolean {
    return !!this.actionLoading?.[c.id]?.[action]
  }

  private setActionLoading(courseId: number, action: RowAction, value: boolean): void {
    this.actionLoading[courseId] = this.actionLoading[courseId] ?? {}
    this.actionLoading[courseId][action] = value
  }

  dismissActionError(): void {
    this.actionError = null
  }

  publish(c: ICours): void {
    if (!this.canPublish(c)) {
      this.actionError = 'Attachez un PDF avant de publier.'
      return
    }
    if (!confirm('Publier ce cours ?')) return

    this.actionError = null
    this.setActionLoading(c.id, 'publish', true)

    this.profCoursSrvc.publishCourse(c.id).subscribe({
      next: () => {
        this.setActionLoading(c.id, 'publish', false)
        this.loadCourses()
      },
      error: (e) => {
        this.setActionLoading(c.id, 'publish', false)
        this.actionError = this.friendly('publish', e)
      },
    })
  }

  unpublish(c: ICours): void {
    if (c.etat !== 'PUBLISHED') return
    if (!confirm('Dépublier ce cours ?')) return

    this.actionError = null
    this.setActionLoading(c.id, 'unpublish', true)

    this.profCoursSrvc.unpublishCourse(c.id).subscribe({
      next: () => {
        this.setActionLoading(c.id, 'unpublish', false)
        this.loadCourses()
      },
      error: (e) => {
        this.setActionLoading(c.id, 'unpublish', false)
        this.actionError = this.friendly('unpublish', e)
      },
    })
  }

  deleteCourse(c: ICours): void {
    if (c.etat !== 'DRAFT') return
    if (!confirm('Supprimer ce cours ?')) return

    this.actionError = null
    this.setActionLoading(c.id, 'delete', true)

    this.profCoursSrvc.deleteCourse(c.id).subscribe({
      next: () => {
        this.setActionLoading(c.id, 'delete', false)
        this.loadCourses()
      },
      error: (e) => {
        this.setActionLoading(c.id, 'delete', false)
        this.actionError = this.friendly('delete', e)
      },
    })
  }

  attachPdf(c: ICours, ev: Event): void {
    const input = ev.target as HTMLInputElement
    const file = input.files?.[0] ?? null
    input.value = ''
    if (!file) return

    this.actionError = null

    this.profCoursSrvc.attachPdf(c.id, file).subscribe({
      next: () => this.loadCourses(),
      error: (e) => {
        this.actionError = this.friendly('publish', e)
      },
    })
  }
}
