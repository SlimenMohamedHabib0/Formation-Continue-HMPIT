import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProfLayout } from './prof-layout/prof-layout';
import { Dashboard } from './dashboard/dashboard';
import { MesCours } from './mes-cours/mes-cours';
import { Inscriptions } from './inscriptions/inscriptions';
import { CoursEditor } from './cours-editor/cours-editor';
import { TestBuilder } from './test-builder/test-builder';

const routes: Routes = [
  {
    path: '',
    component: ProfLayout,
    children: [
      { path: '', component: Dashboard, pathMatch: 'full' },
      { path: 'mes-cours', component: MesCours },
      { path: 'inscriptions', component: Inscriptions },
      { path: 'cours-editor', component: CoursEditor },
      { path: 'cours-editor/:id', component: CoursEditor },
      { path: 'test-builder/:courseId', component: TestBuilder },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ProfSpaceRoutingModule {}
