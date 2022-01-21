# IDM-Aftebi

Requisitos
  Níveis de Produto
  
• Consumíveis (resma de papel, canetas, tinteiros, etc.) → Nivel 0
• Electrónicos (portátil, projector, etc.) → Nivel 1
• Localizações Físicas ou Veículos (salas, automóveis, etc.) → Nível 2

3 tipos de utilizadores, nomeadamente, colaboradores, coordenadores e um administrador.

Critérios de Aprovação de Requisições
• Todas as requisições de nível 0 poderão ser aprovadas automaticamente pelo sistema desde que haja stock0desse produto.
• Todas a requisições de nível 1 feitas por colaboradores terão que ser aprovadas por um coordenador.
• Todas a requisições de nível 1 feitas por coordenadores serão aprovadas automaticamente pelo sistema desde
que haja stock desse produto.
• Todas as requisições de nível 2 feitas por colaboradores terão que ser aprovadas por 2 coordenadores ou pelo
administrador.
• Todas as requisições de nível 2 feitas por coordenadores terão que ser aprovadas por 1 outro coordenador ou
pelo administrador.
• todas as requisições em que não exista stock disponível, a requisição deve passar ao estado de pendente.
